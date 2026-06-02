## Plan: FTP Deploy Angular Dist

You will build the Angular app locally and upload the contents of the generated dist output to Xneelo. Since the site will live at the domain root and uses hash-based routing, it will work on static hosting without special rewrites. We should confirm Supabase settings are correct for production and note that `/api/subscribe` is Vercel-specific and will not exist on a pure FTP host unless you host a backend endpoint.

**Steps**
1. Confirm production config values in [sa-funerals-catalog/src/environments/environment.ts](sa-funerals-catalog/src/environments/environment.ts) for Supabase URL and publishable key; update if the production project differs.
2. Decide how to handle the newsletter endpoint in [sa-funerals-catalog/src/services/marketing.service.ts](sa-funerals-catalog/src/services/marketing.service.ts#L1-L18): keep the existing iframe-only flow, or replace `/api/subscribe` with a real external endpoint since FTP hosting will not provide `/api/*`.
3. Build the app using the default production build in [sa-funerals-catalog/package.json](sa-funerals-catalog/package.json) and verify output lands in dist per [sa-funerals-catalog/angular.json](sa-funerals-catalog/angular.json#L12-L72).
4. Upload the contents of dist (not the dist folder itself) to the Xneelo web root (e.g., public_html). Ensure the generated `/assets`, `/safs-images`, `products-safs.json`, and `wall-chart.html` are present.
5. Confirm routing uses hash-based navigation in [sa-funerals-catalog/src/app.config.ts](sa-funerals-catalog/src/app.config.ts#L1-L16), which avoids the need for server rewrites. Keep any optional rewrite rules only if you later remove hash routing.
6. Smoke test in the browser at `/#/catalog` and `/#/product/{id}` and verify assets load and Supabase queries succeed.

**Verification**
- Run `npm run build` in sa-funerals-catalog and confirm dist contains index.html plus `/assets`, `/safs-images`, and the JSON/HTML assets.
- After FTP upload, open the site and check catalog, product detail, and image assets.
- Trigger any Supabase-backed features to confirm the project URL and key are valid.

**Decisions**
- Base href: root `/`
- Routing: hash-based (no rewrite dependency)
- Vercel image preload: keep as-is for now

If you want, I can refine the plan to include an optional non-hash routing path or a static-friendly newsletter submission flow.
