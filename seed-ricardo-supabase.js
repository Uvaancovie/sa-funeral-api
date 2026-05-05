// seed-ricardo-supabase.js
// Inserts the 29 new Ricardo 2026 products into the Supabase PostgreSQL database
const { Client } = require('pg');
const fs = require('fs');
const path = require('path');

const connectionString = 'postgresql://postgres.hcestxaffzsqlkiedvfx:way2flymillionaire@aws-1-eu-west-1.pooler.supabase.com:5432/postgres';

// Disable SSL certificate verification for Supabase pooler
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

// Load products from products.json
const products = JSON.parse(fs.readFileSync(path.join(__dirname, 'sa-funerals-catalog', 'products.json'), 'utf8'));
const ricardoProducts = products.filter(p => p.id.startsWith('ricardo-'));

// Map of product ID to all related image files (derived from the asset folders)
const casketDir = path.join(__dirname, 'sa-funerals-catalog', 'src', 'assets', 'Ricardo Caskets');
const equipmentDir = path.join(__dirname, 'sa-funerals-catalog', 'src', 'assets', 'Ricardo Equipment');

function getImagesForProduct(product) {
    const primaryImage = product.image;
    const folder = primaryImage.includes('Ricardo Caskets') ? casketDir : equipmentDir;
    const folderName = primaryImage.includes('Ricardo Caskets') ? 'Ricardo Caskets' : 'Ricardo Equipment';

    if (!fs.existsSync(folder)) return [primaryImage];

    // Get primary filename without extension
    const primaryFileName = path.basename(primaryImage, path.extname(primaryImage));

    // Strip trailing color/variant/state words to find the base name
    const suffixWords = ['closed', 'open', 'kiaat', 'white', 'cherry', 'teak',
        'walnut', 'mahogany', 'pecan', 'rose', 'ash', 'black', 'brown', 'green',
        'hemlock', 'red', 'gold', '2', '3', '4'];

    let searchPrefix = primaryFileName;
    let changed = true;
    while (changed) {
        changed = false;
        const trimmed = searchPrefix.trim();
        for (const suffix of suffixWords) {
            if (trimmed.toLowerCase().endsWith(' ' + suffix)) {
                searchPrefix = trimmed.substring(0, trimmed.length - suffix.length - 1).trim();
                changed = true;
                break;
            }
        }
    }

    // Find all matching files
    const allFiles = fs.readdirSync(folder)
        .filter(f => /\.(jpg|jpeg|png)$/i.test(f))
        .filter(f => {
            const fn = path.basename(f, path.extname(f));
            return fn.toLowerCase().startsWith(searchPrefix.toLowerCase()) &&
                !fn.toLowerCase().endsWith(' - copy');
        })
        .sort()
        .map(f => `assets/${folderName}/${f}`);

    return allFiles.length > 0 ? allFiles : [primaryImage];
}

// Color keywords for creating color variations
const colorKeywords = ['cherry', 'teak', 'kiaat', 'walnut', 'white', 'ash', 'black', 'brown', 'green', 'hemlock', 'oak', 'mahogany', 'pine', 'pecan', 'red', 'gold'];

function createColorVariations(variants, images) {
    const colorVariants = variants.filter(v =>
        colorKeywords.some(k => v.toLowerCase().includes(k))
    );

    if (colorVariants.length === 0 || images.length === 0) return null;

    const imagesPerColor = Math.max(1, Math.floor(images.length / Math.max(1, colorVariants.length)));
    const variations = [];

    for (let i = 0; i < colorVariants.length; i++) {
        const colorImages = images.slice(i * imagesPerColor, (i + 1) * imagesPerColor);
        variations.push({
            Color: colorVariants[i],
            Images: colorImages.length > 0 ? colorImages : [images[0]]
        });
    }

    return variations.length > 0 ? JSON.stringify(variations) : null;
}

async function seed() {
    const client = new Client({
        connectionString,
        ssl: { rejectUnauthorized: false }
    });

    try {
        await client.connect();
        console.log('Connected to Supabase PostgreSQL');

        // Check existing Ricardo products
        const existing = await client.query(
            `SELECT "Id" FROM "Products" WHERE "Id" LIKE 'ricardo-%'`
        );
        console.log(`Found ${existing.rows.length} existing Ricardo products in DB`);

        const existingIds = new Set(existing.rows.map(r => r.Id));

        let inserted = 0;
        let skipped = 0;

        for (const product of ricardoProducts) {
            if (existingIds.has(product.id)) {
                console.log(`  ⏭  Skipping (already exists): ${product.name}`);
                skipped++;
                continue;
            }

            const images = getImagesForProduct(product);
            const colorVariations = createColorVariations(product.variants, images);
            const description = `${product.name.replace(' (Ricardo)', '')} available in ${product.variants.join(', ')}`;
            const specifications = JSON.stringify({
                'Available Variants': product.variants.join(', '),
                'Category': product.category === 'casket' ? 'Casket' : product.category === 'accessory' ? 'Accessory' : product.category
            });
            const features = JSON.stringify(product.variants);

            try {
                await client.query(
                    `INSERT INTO "Products" ("Id", "Name", "Category", "Description", "Price", "PriceOnRequest", "Images", "ColorVariations", "Specifications", "Features", "InStock", "Featured", "CreatedAt")
                     VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12, NOW())`,
                    [
                        product.id,
                        product.name,
                        product.category,
                        description,
                        null,           // Price
                        true,           // PriceOnRequest
                        JSON.stringify(images),
                        colorVariations,
                        specifications,
                        features,
                        true,           // InStock
                        false           // Featured
                    ]
                );
                console.log(`  ✓ Inserted: ${product.name} (${images.length} images)`);
                inserted++;
            } catch (err) {
                console.error(`  ✗ Error inserting ${product.name}:`, err.message);
            }
        }

        console.log(`\nDone! Inserted: ${inserted}, Skipped: ${skipped}`);

        // Verify
        const result = await client.query(
            `SELECT "Id", "Name", "Category" FROM "Products" WHERE "Id" LIKE 'ricardo-%' ORDER BY "Category", "Name"`
        );
        console.log(`\nRicardo products now in database: ${result.rows.length}`);
        result.rows.forEach(r => console.log(`  ${r.Category.padEnd(10)} | ${r.Name}`));

    } catch (err) {
        console.error('Connection error:', err.message);
    } finally {
        await client.end();
    }
}

seed();
