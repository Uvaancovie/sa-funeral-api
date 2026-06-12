import re

filepath = r"c:\Users\UvaanG\Desktop\enterprise\sa-funeral-api\sa-funerals-catalog\src\pages\catalog.component.ts"

with open(filepath, "r", encoding="utf-8") as f:
    content = f.read()

replacement_sidebar = """<!-- Filters Sidebar (Desktop) -->
          <div class="hidden lg:block w-[320px] sticky top-[92px] h-[calc(100vh-120px)] overflow-y-auto glass-panel p-6 m-4 rounded-3xl space-y-8 flex-shrink-0" style="contain: layout style;">

            <!-- Desktop Filters Header -->
            <div class="sticky top-0 bg-white/40 z-20 pb-4 mb-4 border-b border-white/20">
              <h2 class="text-xl font-bold text-safs-primary">Filters</h2>
              @if (activeFilterCount() > 0) {
                <button (click)="resetFilters()" class="mt-1 text-safs-accent font-semibold hover:text-safs-primary transition-colors text-sm">
                  Clear all filters
                </button>
              }
            </div>
         <!-- Main Category Filter -->
         <div class="bg-white/40 p-4 rounded-2xl border border-white/20 shadow-sm">
           <h3 class="font-bold text-safs-primary mb-3 uppercase tracking-wider text-xs flex items-center gap-2">
             <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polygon points="22 3 2 3 10 12.46 10 19 14 21 14 12.46 22 3"></polygon></svg>
             Categories
           </h3>
           <div class="space-y-1.5">
             @for (cat of categoryOptions; track cat.value) {
               <button 
                 (click)="selectCategory(cat.value)"
                 [ngClass]="{
                   'bg-safs-primary text-white border-white/40': activeFilter() === cat.value,
                   'text-safs-primary bg-white/60 border-safs-accent/20 hover:bg-white': activeFilter() !== cat.value
                 }"
                 class="w-full text-left px-3.5 py-2.5 rounded-xl font-bold transition-all hover-lift flex justify-between items-center group text-sm border shadow-sm">
                 <span>{{ cat.label }}</span>
                 <span class="text-[10px] font-semibold px-2 py-0.5 rounded-full" [ngClass]="activeFilter() === cat.value ? 'bg-white/20 text-white' : 'bg-safs-primary/10 text-safs-primary'">{{ getCategoryCount(cat.value) }}</span>
               </button>
             }
           </div>
         </div>

         <!-- Product Style Filter -->
         @if (showStyleFilter()) {
           <div class="bg-white/40 p-4 rounded-2xl border border-white/20 shadow-sm animate-fade-in">
             <h3 class="font-bold text-safs-primary mb-3 uppercase tracking-wider text-xs">Product Styles</h3>
             <div class="flex flex-wrap gap-1.5">
               @for (style of casketStyles; track style) {
                 <button 
                   (click)="toggleStyle(style)"
                   [class.bg-safs-primary]="selectedStyles().includes(style)"
                   [class.text-white]="selectedStyles().includes(style)"
                   [class.bg-white/80]="!selectedStyles().includes(style)"
                   [class.text-safs-primary]="!selectedStyles().includes(style)"
                   class="px-3 py-2 rounded-xl text-xs font-semibold transition-all hover-lift border border-white/40 shadow-sm">
                   {{ style }}
                 </button>
               }
             </div>
           </div>
         }

         <!-- Color/Finish Filter -->
         <div class="bg-white/40 p-4 rounded-2xl border border-white/20 shadow-sm mb-6">
           <h3 class="font-bold text-safs-primary mb-3 uppercase tracking-wider text-xs">Finishes & Colors</h3>
           <div class="grid grid-cols-1 gap-2">
             @for (finish of availableFinishes; track finish) {
               <button 
                 (click)="toggleFinish(finish)"
                 [ngClass]="{
                   'border-safs-accent bg-safs-primary/5': selectedFinishes().includes(finish),
                   'border-white/40 bg-white/70': !selectedFinishes().includes(finish)
                 }"
                 class="flex items-center gap-3 px-4 py-2 rounded-xl hover:bg-white transition-all text-sm font-medium shadow-sm border group hover-lift">
                 <div [style.background-color]="getFinishColor(finish)" class="w-4 h-4 rounded-full border border-gray-300 shadow-inner shrink-0"></div>
                 <span class="text-safs-primary group-hover:text-safs-accent text-left whitespace-normal break-words leading-tight font-medium">{{ finish }}</span>
               </button>
             }
           </div>
         </div>

            <button (click)="resetFilters()" class="w-full py-3 text-safs-accent font-bold text-sm uppercase tracking-widest hover:bg-white/40 rounded-xl border border-white/20 transition-colors mb-6">
              Clear All Filters
            </button>
          </div>
         """

pattern = r"<!--\s*Filters\s+Sidebar\s+\(Desktop\)\s*-->.*?<!--\s*Mobile\s+Filters\s+Modal\s*-->"

new_content, count = re.subn(pattern, replacement_sidebar + "\\n         <!-- Mobile Filters Modal -->", content, flags=re.DOTALL)

if count > 0:
    with open(filepath, "w", encoding="utf-8") as f:
        f.write(new_content)
    print(f"SUCCESS: Replaced {count} occurrences")
else:
    print("PATTERN NOT MATCHED")
