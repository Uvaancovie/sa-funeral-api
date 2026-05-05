// create-orders-table.js
// Creates the Orders table directly in Supabase
const { Client } = require('pg');

const connectionString = 'postgresql://postgres.hcestxaffzsqlkiedvfx:way2flymillionaire@aws-1-eu-west-1.pooler.supabase.com:5432/postgres';
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

async function createTable() {
    const client = new Client({ connectionString, ssl: { rejectUnauthorized: false } });
    try {
        await client.connect();
        console.log('Connected to Supabase');

        await client.query(`
            CREATE TABLE IF NOT EXISTS "Orders" (
                "OrderId"        SERIAL PRIMARY KEY,
                "CustomerId"     INTEGER NOT NULL,
                "CustomerEmail"  VARCHAR(255) NOT NULL,
                "CustomerCompany" VARCHAR(255),
                "CustomerContact" VARCHAR(255),
                "Items"          TEXT NOT NULL DEFAULT '[]',
                "Status"         VARCHAR(50) NOT NULL DEFAULT 'pending',
                "Notes"          TEXT,
                "CreatedAt"      TIMESTAMPTZ NOT NULL DEFAULT now(),
                "UpdatedAt"      TIMESTAMPTZ
            );
        `);
        console.log('✓ Orders table created (or already exists)');

        await client.query(`CREATE INDEX IF NOT EXISTS "IX_Orders_CustomerId" ON "Orders" ("CustomerId");`);
        await client.query(`CREATE INDEX IF NOT EXISTS "IX_Orders_Status" ON "Orders" ("Status");`);
        await client.query(`CREATE INDEX IF NOT EXISTS "IX_Orders_CreatedAt" ON "Orders" ("CreatedAt");`);
        console.log('✓ Indexes created');

        // Verify
        const result = await client.query(`SELECT COUNT(*) FROM "Orders"`);
        console.log(`\n✓ Orders table ready. Current row count: ${result.rows[0].count}`);

    } catch (err) {
        console.error('Error:', err.message);
    } finally {
        await client.end();
    }
}

createTable();
