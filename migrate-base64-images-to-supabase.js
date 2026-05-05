#!/usr/bin/env node

/**
 * Migration Script: Convert Base64 Product Images to Supabase Storage
 *
 * This script:
 * 1. Connects to Supabase database
 * 2. Queries all products with Base64 images
 * 3. Converts Base64 to binary and uploads to Supabase Storage
 * 4. Updates product records with new public URLs
 *
 * Usage: node migrate-base64-images-to-supabase.js
 *
 * Make sure you have SUPABASE_URL and SUPABASE_KEY set in .env
 */

const { createClient } = require('@supabase/supabase-js');
const fs = require('fs');
const path = require('path');
require('dotenv').config();

const SUPABASE_URL = process.env.SUPABASE_URL;
const SUPABASE_KEY = process.env.SUPABASE_KEY;
const BUCKET_NAME = 'product-images';

if (!SUPABASE_URL || !SUPABASE_KEY) {
    console.error('❌ Error: SUPABASE_URL and SUPABASE_KEY environment variables are required');
    process.exit(1);
}

// Initialize Supabase client
const supabase = createClient(SUPABASE_URL, SUPABASE_KEY);

/**
 * Check if a string is a Base64 data URI
 */
function isBase64DataUri(str) {
    if (typeof str !== 'string') return false;
    return str.startsWith('data:image/');
}

/**
 * Convert Base64 data URI to Buffer
 */
function base64ToBuffer(dataUri) {
    try {
        // Extract Base64 part (after comma)
        const base64String = dataUri.split(',')[1];
        if (!base64String) {
            console.warn('⚠️  Invalid Base64 data URI format');
            return null;
        }
        return Buffer.from(base64String, 'base64');
    } catch (error) {
        console.error('❌ Error converting Base64 to buffer:', error.message);
        return null;
    }
}

/**
 * Get file extension from Base64 data URI
 */
function getFileExtension(dataUri) {
    const match = dataUri.match(/data:image\/(\w+)/);
    return match ? match[1] : 'jpg';
}

/**
 * Upload Base64 image to Supabase Storage
 */
async function uploadBase64ToSupabase(base64DataUri, productId, folder = 'base') {
    try {
        const fileExt = getFileExtension(base64DataUri);
        const fileName = `${productId}/${folder}/${Math.random().toString(36).substring(2, 15)}.${fileExt}`;
        const buffer = base64ToBuffer(base64DataUri);

        if (!buffer) {
            console.warn(`⚠️  Failed to convert Base64 for ${fileName}`);
            return null;
        }

        // Upload to Supabase Storage
        const { data, error } = await supabase.storage
            .from(BUCKET_NAME)
            .upload(fileName, buffer, {
                cacheControl: '3600',
                upsert: true,
                contentType: `image/${fileExt}`
            });

        if (error) {
            console.error(`❌ Upload error for ${fileName}:`, error.message);
            return null;
        }

        // Get public URL
        const { data: publicUrlData } = supabase.storage
            .from(BUCKET_NAME)
            .getPublicUrl(fileName);

        console.log(`✅ Uploaded: ${fileName}`);
        return publicUrlData.publicUrl;
    } catch (error) {
        console.error('❌ Error uploading Base64 image:', error.message);
        return null;
    }
}

/**
 * Migrate product images
 */
async function migrateProductImages() {
    try {
        console.log('🔄 Starting migration of Base64 images to Supabase Storage...\n');

        // Fetch all products
        const { data: products, error: fetchError } = await supabase
            .from('Products')
            .select('*');

        if (fetchError) {
            console.error('❌ Error fetching products:', fetchError.message);
            process.exit(1);
        }

        console.log(`📦 Found ${products.length} products\n`);

        let totalMigrated = 0;
        let totalBase64 = 0;

        // Process each product
        for (const product of products) {
            try {
                let images = [];
                try {
                    images = JSON.parse(product.Images || '[]');
                } catch (e) {
                    console.warn(`⚠️  Invalid JSON for product ${product.Id}: ${e.message}`);
                    continue;
                }

                if (!images.length) {
                    continue;
                }

                let hasChanges = false;
                const migratedImages = [];

                // Process each image
                for (let i = 0; i < images.length; i++) {
                    const imageUrl = images[i];

                    if (isBase64DataUri(imageUrl)) {
                        totalBase64++;
                        console.log(`\n🖼️  Migrating product "${product.Name}" (ID: ${product.Id}) - image ${i + 1}/${images.length}`);

                        const publicUrl = await uploadBase64ToSupabase(imageUrl, product.Id, 'base');

                        if (publicUrl) {
                            migratedImages.push(publicUrl);
                            hasChanges = true;
                            totalMigrated++;
                            console.log(`   Updated with URL: ${publicUrl}`);
                        } else {
                            // Keep original if migration failed
                            migratedImages.push(imageUrl);
                        }
                    } else {
                        // Already a public URL, keep as is
                        migratedImages.push(imageUrl);
                    }

                    // Add a small delay to avoid rate limiting
                    await new Promise(resolve => setTimeout(resolve, 200));
                }

                // Update product with new URLs if there were changes
                if (hasChanges) {
                    const { error: updateError } = await supabase
                        .from('Products')
                        .update({ Images: JSON.stringify(migratedImages) })
                        .eq('Id', product.Id);

                    if (updateError) {
                        console.error(`❌ Error updating product ${product.Id}:`, updateError.message);
                    } else {
                        console.log(`✅ Updated product ${product.Id} in database`);
                    }
                }
            } catch (error) {
                console.error(`❌ Error processing product ${product.Id}:`, error.message);
            }
        }

        // Migrate color variations
        console.log('\n\n📦 Processing color variations...');
        for (const product of products) {
            try {
                let colorVariations = [];
                try {
                    colorVariations = JSON.parse(product.ColorVariations || '[]');
                } catch (e) {
                    continue;
                }

                if (!colorVariations.length) {
                    continue;
                }

                let hasChanges = false;

                // Process each color variation
                for (const variation of colorVariations) {
                    const migratedColorImages = [];

                    for (let i = 0; i < variation.Images.length; i++) {
                        const imageUrl = variation.Images[i];

                        if (isBase64DataUri(imageUrl)) {
                            totalBase64++;
                            console.log(`\n🎨 Migrating color "${variation.Color}" of "${product.Name}" - image ${i + 1}/${variation.Images.length}`);

                            const publicUrl = await uploadBase64ToSupabase(imageUrl, product.Id, `color-${variation.Color.replace(/\s+/g, '-').toLowerCase()}`);

                            if (publicUrl) {
                                migratedColorImages.push(publicUrl);
                                hasChanges = true;
                                totalMigrated++;
                                console.log(`   Updated with URL: ${publicUrl}`);
                            } else {
                                migratedColorImages.push(imageUrl);
                            }
                        } else {
                            migratedColorImages.push(imageUrl);
                        }

                        await new Promise(resolve => setTimeout(resolve, 200));
                    }

                    variation.Images = migratedColorImages;
                }

                // Update product if there were changes
                if (hasChanges) {
                    const { error: updateError } = await supabase
                        .from('Products')
                        .update({ ColorVariations: JSON.stringify(colorVariations) })
                        .eq('Id', product.Id);

                    if (updateError) {
                        console.error(`❌ Error updating color variations for ${product.Id}:`, updateError.message);
                    } else {
                        console.log(`✅ Updated color variations for ${product.Id} in database`);
                    }
                }
            } catch (error) {
                console.error(`❌ Error processing color variations for ${product.Id}:`, error.message);
            }
        }

        // Summary
        console.log('\n\n' + '='.repeat(50));
        console.log('📊 Migration Summary');
        console.log('='.repeat(50));
        console.log(`Total Base64 images found: ${totalBase64}`);
        console.log(`Successfully migrated to Supabase: ${totalMigrated}`);
        console.log(`Failed migrations: ${totalBase64 - totalMigrated}`);
        console.log('='.repeat(50) + '\n');

        if (totalMigrated > 0) {
            console.log('✅ Migration complete! All Base64 images have been converted to Supabase URLs.');
        } else {
            console.log('⚠️  No Base64 images found to migrate.');
        }

    } catch (error) {
        console.error('❌ Fatal error during migration:', error.message);
        process.exit(1);
    }
}

// Run migration
migrateProductImages();
