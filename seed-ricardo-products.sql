-- ============================================================
-- Ricardo 2026 Products - Insert into Supabase
-- Run this script against the Supabase PostgreSQL database
-- ============================================================

-- ==================== RICARDO CASKETS ====================

INSERT INTO "Products" ("Id", "Name", "Category", "Description", "Price", "PriceOnRequest", "Images", "ColorVariations", "Specifications", "Features", "InStock", "Featured", "CreatedAt")
VALUES
-- 1. 2 Tier Oversize Casket
('ricardo-2-tier-oversize', '2 Tier Oversize Casket', 'casket',
 '2 Tier Oversize Casket available in Kiaat, White',
 NULL, true,
 '["assets/Ricardo Caskets/2 Tier Oversize Kiaat.jpg","assets/Ricardo Caskets/2 Tier Oversize White.jpg"]',
 '[{"Color":"Kiaat","Images":["assets/Ricardo Caskets/2 Tier Oversize Kiaat.jpg"]},{"Color":"White","Images":["assets/Ricardo Caskets/2 Tier Oversize White.jpg"]}]',
 '{"Available Variants":"Kiaat, White","Category":"Casket"}',
 '["Kiaat","White"]',
 true, false, NOW()),

-- 2. 2FT Raised Halfview
('ricardo-2ft-raised-halfview', '2FT Raised Halfview', 'casket',
 '2FT Raised Halfview available in White',
 NULL, true,
 '["assets/Ricardo Caskets/2FT Raised Halfview White.jpg"]',
 '[{"Color":"White","Images":["assets/Ricardo Caskets/2FT Raised Halfview White.jpg"]}]',
 '{"Available Variants":"White","Category":"Casket"}',
 '["White"]',
 true, false, NOW()),

-- 3. 3FT Raised Halfview
('ricardo-3ft-raised-halfview', '3FT Raised Halfview', 'casket',
 '3FT Raised Halfview available in White',
 NULL, true,
 '["assets/Ricardo Caskets/3FT Raised Halfview White.jpg"]',
 '[{"Color":"White","Images":["assets/Ricardo Caskets/3FT Raised Halfview White.jpg"]}]',
 '{"Available Variants":"White","Category":"Casket"}',
 '["White"]',
 true, false, NOW()),

-- 4. 4FT Raised Halfview
('ricardo-4ft-raised-halfview', '4FT Raised Halfview', 'casket',
 '4FT Raised Halfview available in White',
 NULL, true,
 '["assets/Ricardo Caskets/4FT Raised Halfview White.jpg"]',
 '[{"Color":"White","Images":["assets/Ricardo Caskets/4FT Raised Halfview White.jpg"]}]',
 '{"Available Variants":"White","Category":"Casket"}',
 '["White"]',
 true, false, NOW()),

-- 5. 4 Corner Woodturning Casket (Ricardo)
('ricardo-4-corner-woodturning', '4 Corner Woodturning Casket (Ricardo)', 'casket',
 '4 Corner Woodturning Casket available in Teak',
 NULL, true,
 '["assets/Ricardo Caskets/4 Corner Woodturning Teak.jpg"]',
 '[{"Color":"Teak","Images":["assets/Ricardo Caskets/4 Corner Woodturning Teak.jpg"]}]',
 '{"Available Variants":"Teak","Category":"Casket"}',
 '["Teak"]',
 true, false, NOW()),

-- 6. C-Coffin
('ricardo-c-coffin', 'C-Coffin', 'casket',
 'C-Coffin available in Cherry',
 NULL, true,
 '["assets/Ricardo Caskets/C-Coffin Cherry Closed.jpg","assets/Ricardo Caskets/C-Coffin Cherry Open.jpg"]',
 '[{"Color":"Cherry","Images":["assets/Ricardo Caskets/C-Coffin Cherry Closed.jpg","assets/Ricardo Caskets/C-Coffin Cherry Open.jpg"]}]',
 '{"Available Variants":"Cherry","Category":"Casket"}',
 '["Cherry"]',
 true, false, NOW()),

-- 7. C-Corner Coffin
('ricardo-c-corner-coffin', 'C-Corner Coffin', 'casket',
 'C-Corner Coffin available in White',
 NULL, true,
 '["assets/Ricardo Caskets/C-Corner Coffin White.jpg"]',
 '[{"Color":"White","Images":["assets/Ricardo Caskets/C-Corner Coffin White.jpg"]}]',
 '{"Available Variants":"White","Category":"Casket"}',
 '["White"]',
 true, false, NOW()),

-- 8. Classic Coffin
('ricardo-classic-coffin', 'Classic Coffin', 'casket',
 'Classic Coffin available in Mahogany, Mahogany Rose, Pecan, Pecan Rose',
 NULL, true,
 '["assets/Ricardo Caskets/Classic Coffin Mahogany.jpg","assets/Ricardo Caskets/Classic Coffin Mahogany Rose Closed.jpg","assets/Ricardo Caskets/Classic Coffin Mahogany Rose Open.jpg","assets/Ricardo Caskets/Classic Coffin Pecan Closed.jpg","assets/Ricardo Caskets/Classic Coffin Pecan Open.jpg","assets/Ricardo Caskets/Classic Coffin Pecan Rose.jpg"]',
 '[{"Color":"Mahogany","Images":["assets/Ricardo Caskets/Classic Coffin Mahogany.jpg"]},{"Color":"Mahogany Rose","Images":["assets/Ricardo Caskets/Classic Coffin Mahogany Rose Closed.jpg","assets/Ricardo Caskets/Classic Coffin Mahogany Rose Open.jpg"]},{"Color":"Pecan","Images":["assets/Ricardo Caskets/Classic Coffin Pecan Closed.jpg","assets/Ricardo Caskets/Classic Coffin Pecan Open.jpg"]},{"Color":"Pecan Rose","Images":["assets/Ricardo Caskets/Classic Coffin Pecan Rose.jpg"]}]',
 '{"Available Variants":"Mahogany, Mahogany Rose, Pecan, Pecan Rose","Category":"Casket"}',
 '["Mahogany","Mahogany Rose","Pecan","Pecan Rose"]',
 true, false, NOW()),

-- 9. Flatlid Coffin
('ricardo-flatlid-coffin', 'Flatlid Coffin', 'casket',
 'Flatlid Coffin available in Kiaat, White',
 NULL, true,
 '["assets/Ricardo Caskets/Flatlid Coffin Kiaat.jpg","assets/Ricardo Caskets/Flatlid Coffin White.jpg"]',
 '[{"Color":"Kiaat","Images":["assets/Ricardo Caskets/Flatlid Coffin Kiaat.jpg"]},{"Color":"White","Images":["assets/Ricardo Caskets/Flatlid Coffin White.jpg"]}]',
 '{"Available Variants":"Kiaat, White","Category":"Casket"}',
 '["Kiaat","White"]',
 true, false, NOW()),

-- 10. Harvard Coffin (Ricardo)
('ricardo-harvard-coffin', 'Harvard Coffin (Ricardo)', 'casket',
 'Harvard Coffin available in Cherry, White',
 NULL, true,
 '["assets/Ricardo Caskets/Harvard Coffin Cherry Closed.jpg","assets/Ricardo Caskets/Harvard Coffin Cherry Open.jpg","assets/Ricardo Caskets/Harvard Coffin White.jpg"]',
 '[{"Color":"Cherry","Images":["assets/Ricardo Caskets/Harvard Coffin Cherry Closed.jpg","assets/Ricardo Caskets/Harvard Coffin Cherry Open.jpg"]},{"Color":"White","Images":["assets/Ricardo Caskets/Harvard Coffin White.jpg"]}]',
 '{"Available Variants":"Cherry, White","Category":"Casket"}',
 '["Cherry","White"]',
 true, false, NOW()),

-- 11. Lincoln Coffin (Ricardo)
('ricardo-lincoln-coffin', 'Lincoln Coffin (Ricardo)', 'casket',
 'Lincoln Coffin available in White',
 NULL, true,
 '["assets/Ricardo Caskets/Lincoln Coffin White Closed.jpg","assets/Ricardo Caskets/Lincoln Coffin White Open.jpg"]',
 '[{"Color":"White","Images":["assets/Ricardo Caskets/Lincoln Coffin White Closed.jpg","assets/Ricardo Caskets/Lincoln Coffin White Open.jpg"]}]',
 '{"Available Variants":"White","Category":"Casket"}',
 '["White"]',
 true, false, NOW()),

-- 12. Oxford Casket (Ricardo)
('ricardo-oxford', 'Oxford Casket (Ricardo)', 'casket',
 'Oxford Casket available in Cherry',
 NULL, true,
 '["assets/Ricardo Caskets/Oxford Cherry Closed.jpg","assets/Ricardo Caskets/Oxford Cherry Open.jpg"]',
 '[{"Color":"Cherry","Images":["assets/Ricardo Caskets/Oxford Cherry Closed.jpg","assets/Ricardo Caskets/Oxford Cherry Open.jpg"]}]',
 '{"Available Variants":"Cherry","Category":"Casket"}',
 '["Cherry"]',
 true, false, NOW()),

-- 13. Pongee Casket
('ricardo-pongee', 'Pongee Casket', 'casket',
 'Pongee Casket available in Kiaat',
 NULL, true,
 '["assets/Ricardo Caskets/Pongee Kiaat.jpg"]',
 '[{"Color":"Kiaat","Images":["assets/Ricardo Caskets/Pongee Kiaat.jpg"]}]',
 '{"Available Variants":"Kiaat","Category":"Casket"}',
 '["Kiaat"]',
 true, false, NOW()),

-- 14. Prestige Coffin
('ricardo-prestige-coffin', 'Prestige Coffin', 'casket',
 'Prestige Coffin available in Mahogany, Pecan, White',
 NULL, true,
 '["assets/Ricardo Caskets/Prestige Coffin Mahogany Closed.jpg","assets/Ricardo Caskets/Prestige Coffin Mahogany open.jpg","assets/Ricardo Caskets/Prestige Coffin Pecan Closed.jpg","assets/Ricardo Caskets/Prestige Coffin Pecan Open.jpg","assets/Ricardo Caskets/Prestige Coffin White Closed.jpg","assets/Ricardo Caskets/Prestige Coffin White Open.jpg"]',
 '[{"Color":"Mahogany","Images":["assets/Ricardo Caskets/Prestige Coffin Mahogany Closed.jpg","assets/Ricardo Caskets/Prestige Coffin Mahogany open.jpg"]},{"Color":"Pecan","Images":["assets/Ricardo Caskets/Prestige Coffin Pecan Closed.jpg","assets/Ricardo Caskets/Prestige Coffin Pecan Open.jpg"]},{"Color":"White","Images":["assets/Ricardo Caskets/Prestige Coffin White Closed.jpg","assets/Ricardo Caskets/Prestige Coffin White Open.jpg"]}]',
 '{"Available Variants":"Mahogany, Pecan, White","Category":"Casket"}',
 '["Mahogany","Pecan","White"]',
 true, false, NOW()),

-- 15. Redwood Coffin
('ricardo-redwood-coffin', 'Redwood Coffin', 'casket',
 'Redwood Coffin',
 NULL, true,
 '["assets/Ricardo Caskets/Redwood Coffin.jpg"]',
 NULL,
 '{"Available Variants":"Standard","Category":"Casket"}',
 '["Standard"]',
 true, false, NOW()),

-- 16. Repatriation Casket
('ricardo-repatriation-casket', 'Repatriation Casket', 'casket',
 'Repatriation Casket',
 NULL, true,
 '["assets/Ricardo Caskets/Repatriation Casket Closed.jpg","assets/Ricardo Caskets/Repatriation Casket Open.jpg"]',
 NULL,
 '{"Available Variants":"Standard","Category":"Casket"}',
 '["Standard"]',
 true, false, NOW()),

-- 17. Senator Coffin (Ricardo)
('ricardo-senator-coffin', 'Senator Coffin (Ricardo)', 'casket',
 'Senator Coffin available in White',
 NULL, true,
 '["assets/Ricardo Caskets/Senator Coffin White Closed.jpg","assets/Ricardo Caskets/Senator Coffin White Open.jpg"]',
 '[{"Color":"White","Images":["assets/Ricardo Caskets/Senator Coffin White Closed.jpg","assets/Ricardo Caskets/Senator Coffin White Open.jpg"]}]',
 '{"Available Variants":"White","Category":"Casket"}',
 '["White"]',
 true, false, NOW()),

-- 18. Solid Teak Casket
('ricardo-solid-teak-casket', 'Solid Teak Casket', 'casket',
 'Solid Teak Casket available in Teak',
 NULL, true,
 '["assets/Ricardo Caskets/Solid Teak Casket Closed.jpg","assets/Ricardo Caskets/Solid Teak Casket Open.jpg"]',
 '[{"Color":"Teak","Images":["assets/Ricardo Caskets/Solid Teak Casket Closed.jpg","assets/Ricardo Caskets/Solid Teak Casket Open.jpg"]}]',
 '{"Available Variants":"Teak","Category":"Casket"}',
 '["Teak"]',
 true, false, NOW()),

-- 19. Top End Coffin
('ricardo-top-end-coffin', 'Top End Coffin', 'casket',
 'Top End Coffin available in Cherry, Kiaat, White',
 NULL, true,
 '["assets/Ricardo Caskets/Top End Coffin Cherry.jpg","assets/Ricardo Caskets/Top End Coffin Kiaat Closed.jpg","assets/Ricardo Caskets/Top End Coffin Kiaat Open.jpg","assets/Ricardo Caskets/Top End Coffin White Closed.jpg","assets/Ricardo Caskets/Top End Coffin White Open.jpg"]',
 '[{"Color":"Cherry","Images":["assets/Ricardo Caskets/Top End Coffin Cherry.jpg"]},{"Color":"Kiaat","Images":["assets/Ricardo Caskets/Top End Coffin Kiaat Closed.jpg","assets/Ricardo Caskets/Top End Coffin Kiaat Open.jpg"]},{"Color":"White","Images":["assets/Ricardo Caskets/Top End Coffin White Closed.jpg","assets/Ricardo Caskets/Top End Coffin White Open.jpg"]}]',
 '{"Available Variants":"Cherry, Kiaat, White","Category":"Casket"}',
 '["Cherry","Kiaat","White"]',
 true, false, NOW()),

-- ==================== RICARDO EQUIPMENT ====================

-- 20. 1 Man Stretcher
('ricardo-1-man-stretcher', '1 Man Stretcher', 'accessory',
 '1 Man Stretcher available in Red',
 NULL, true,
 '["assets/Ricardo Equipment/1 Man Stretcher Red.jpg"]',
 '[{"Color":"Red","Images":["assets/Ricardo Equipment/1 Man Stretcher Red.jpg"]}]',
 '{"Available Variants":"Red","Category":"Accessory"}',
 '["Red"]',
 true, false, NOW()),

-- 21. Body Box (Ricardo)
('ricardo-body-box', 'Body Box (Ricardo)', 'accessory',
 'Body Box',
 NULL, true,
 '["assets/Ricardo Equipment/Body Box.jpg","assets/Ricardo Equipment/Body Box 2.jpg"]',
 NULL,
 '{"Available Variants":"Standard","Category":"Accessory"}',
 '["Standard"]',
 true, false, NOW()),

-- 22. Casket Racking System (Ricardo)
('ricardo-casket-racking-system', 'Casket Racking System (Ricardo)', 'accessory',
 'Casket Racking System',
 NULL, true,
 '["assets/Ricardo Equipment/Casket Racking System.jpg"]',
 NULL,
 '{"Available Variants":"Standard","Category":"Accessory"}',
 '["Standard"]',
 true, false, NOW()),

-- 23. Coffin Stand (Ricardo)
('ricardo-coffin-stand', 'Coffin Stand (Ricardo)', 'accessory',
 'Coffin Stand',
 NULL, true,
 '["assets/Ricardo Equipment/Coffin Stand.jpg"]',
 NULL,
 '{"Available Variants":"Standard","Category":"Accessory"}',
 '["Standard"]',
 true, false, NOW()),

-- 24. Executive Church Trolley
('ricardo-executive-church-trolley', 'Executive Church Trolley', 'accessory',
 'Executive Church Trolley available in Gold',
 NULL, true,
 '["assets/Ricardo Equipment/Executive Church Trolley Gold.jpg"]',
 '[{"Color":"Gold","Images":["assets/Ricardo Equipment/Executive Church Trolley Gold.jpg"]}]',
 '{"Available Variants":"Gold","Category":"Accessory"}',
 '["Gold"]',
 true, false, NOW()),

-- 25. Grassmats (Ricardo)
('ricardo-grassmats', 'Grassmats (Ricardo)', 'accessory',
 'Grassmats',
 NULL, true,
 '["assets/Ricardo Equipment/Grassmats.jpg"]',
 NULL,
 '{"Available Variants":"Standard","Category":"Accessory"}',
 '["Standard"]',
 true, false, NOW()),

-- 26. Green Tent 3x6
('ricardo-green-tent', 'Green Tent 3x6', 'accessory',
 'Green Tent 3x6',
 NULL, true,
 '["assets/Ricardo Equipment/Green Tent 3x6.jpg"]',
 NULL,
 '{"Available Variants":"3x6","Category":"Accessory"}',
 '["3x6"]',
 true, false, NOW()),

-- 27. Head Block
('ricardo-head-block', 'Head Block', 'accessory',
 'Head Block',
 NULL, true,
 '["assets/Ricardo Equipment/Head Block.jpg"]',
 NULL,
 '{"Available Variants":"Standard","Category":"Accessory"}',
 '["Standard"]',
 true, false, NOW()),

-- 28. Lowering Device Set (Ricardo)
('ricardo-lowering-device-set', 'Lowering Device Set (Ricardo)', 'accessory',
 'Lowering Device Set',
 NULL, true,
 '["assets/Ricardo Equipment/Lowering Device Set.jpg"]',
 NULL,
 '{"Available Variants":"Standard","Category":"Accessory"}',
 '["Standard"]',
 true, false, NOW()),

-- 29. Tent, Grassmat & Lowering Device Set
('ricardo-tent-grassmat-lowdev-set', 'Tent, Grassmat & Lowering Device Set', 'accessory',
 'Tent, Grassmat & Lowering Device Set',
 NULL, true,
 '["assets/Ricardo Equipment/Tent,Grassmat & Lowdev Set.jpg"]',
 NULL,
 '{"Available Variants":"Standard","Category":"Accessory"}',
 '["Standard"]',
 true, false, NOW())

ON CONFLICT ("Id") DO NOTHING;

-- Verify insertion
SELECT "Id", "Name", "Category" FROM "Products" WHERE "Id" LIKE 'ricardo-%' ORDER BY "Category", "Name";
