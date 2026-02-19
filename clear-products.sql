-- Clear all products from the database
DELETE FROM Products;

-- Reset the identity seed
DBCC CHECKIDENT ('Products', RESEED, 0);

SELECT 'Products table cleared' AS Status;
