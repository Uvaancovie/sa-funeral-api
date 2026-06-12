import re

filepath = r"c:\Users\UvaanG\Desktop\enterprise\sa-funeral-api\sa-funerals-catalog\src\pages\catalog.component.ts"

with open(filepath, "r", encoding="utf-8") as f:
    content = f.read()

replacement_grid = """<!-- Grid -->
            <div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4 sm:gap-5 md:gap-6 lg:gap-8">
             @for (product of filteredProducts(); track product.id) {
               <div data-testid="catalog-card" class="glass-card hover-lift rounded-3xl overflow-hidden group flex flex-col cursor-pointer border border-white/40 shadow-sm" [routerLink]="['/product', product.id]">
                 
                  <!-- Image Area -->
                  <div class="relative h-64 sm:h-72 md:h-80 lg:h-90 overflow-hidden bg-white/80 border-b border-white/20">
                    <app-optimized-image
                      [src]="getOptimizedProductImagePath(product)"
                      [alt]="product.name"
                      [aspectRatio]="getProductAspectRatio(product)"
                      [loading]="getImageLoadingStrategy($index, filteredProducts().length, $first)"
                      [fetchpriority]="getImageFetchPriority($index, $first)"
                      containerClass="group-hover:scale-105 transition-transform duration-700 ease-out"
                    ></app-optimized-image>
                   
                   <div class="absolute inset-0 transition-opacity flex items-center justify-center opacity-0 group-hover:opacity-100 bg-safs-primary/10 backdrop-blur-[2px]">
                       <span class="bg-safs-primary text-white font-bold py-2.5 px-6 rounded-xl shadow-xl transform translate-y-4 group-hover:translate-y-0 transition-all duration-300 text-sm flex items-center gap-2 border border-white/20">
                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M2 12s3-7 10-7 10 7 10 7-3 7-10 7-10-7-10-7Z"/><circle cx="12" cy="12" r="3"/></svg>
                         View Details
                     </span>
                   </div>
                   
                   @if (getProductColorVariations(product).length > 0) {
                     <div class="absolute bottom-3 left-3 inline-flex px-2.5 py-1 rounded-xl bg-white/95 backdrop-blur-md shadow-md items-center gap-1.5 border border-white/20">
                       <div class="flex -space-x-1.5">
                         @for (variation of getProductColorVariations(product).slice(0, 3); track variation.color) {
                           <div [style.background-color]="getFinishColor(variation.color)" class="w-4 h-4 rounded-full border border-white shadow-sm"></div>
                         }
                       </div>
                       <span class="text-[10px] font-bold text-safs-primary pl-1">{{ getProductColorVariations(product).length }} Colors</span>
                     </div>
                   }
                 </div>
 
                  <!-- Content -->
                  <div class="p-5 flex-1 flex flex-col glass-card-inner border-t border-white/20">
                    <div class="text-[10px] font-black text-safs-accent uppercase tracking-[0.2em] mb-1.5">{{ getCategoryDisplayName(product.category) }}</div>
                    <h2 class="text-base font-bold text-safs-primary group-hover:text-safs-accent transition-colors leading-tight mb-2">{{ product.name }}</h2>
                   
                   <div class="mt-auto">
                     <!-- Actions removed -->
                   </div>
                 </div>
               </div>
             }
           </div>
        """

pattern = r"<!--\s*Grid\s*-->.*?@if\s*\(\s*filteredProducts\(\)\.length\s*===\s*0\s*\)\s*{"

new_content, count = re.subn(pattern, replacement_grid + "\n\n          @if (filteredProducts().length === 0) {", content, flags=re.DOTALL)

if count > 0:
    with open(filepath, "w", encoding="utf-8") as f:
        f.write(new_content)
    print(f"SUCCESS: Replaced {count} occurrences")
else:
    print("PATTERN NOT MATCHED")
