// rename-ricardo-products.js
// Strips " (Ricardo)" from product names in Supabase
const { Client } = require('pg');

const connectionString = 'postgresql://postgres.hcestxaffzsqlkiedvfx:way2flymillionaire@aws-1-eu-west-1.pooler.supabase.com:5432/postgres';
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

async function rename() {
    const client = new Client({
        connectionString,
        ssl: { rejectUnauthorized: false }
    });

    try {
        await client.connect();
        console.log('Connected to Supabase');

        // Update names, removing " (Ricardo)" suffix
        const result = await client.query(`
            UPDATE "Products"
            SET "Name" = REPLACE("Name", ' (Ricardo)', '')
            WHERE "Name" LIKE '% (Ricardo)%'
            RETURNING "Id", "Name"
        `);

        console.log(`\nRenamed ${result.rowCount} products:`);
        result.rows.forEach(r => console.log(`  ✓ ${r.Id}: ${r.Name}`));

        // Also update products.json description fields
        const allRicardo = await client.query(`
            SELECT "Id", "Name" FROM "Products" WHERE "Id" LIKE 'ricardo-%' ORDER BY "Name"
        `);
        console.log(`\nAll Ricardo products (${allRicardo.rowCount}):`);
        allRicardo.rows.forEach(r => console.log(`  ${r.Name}`));

    } catch (err) {
        console.error('Error:', err.message);
    } finally {
        await client.end();
    }
}

rename();
