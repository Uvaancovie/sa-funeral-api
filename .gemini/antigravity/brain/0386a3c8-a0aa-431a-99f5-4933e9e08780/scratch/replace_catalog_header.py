import re

filepath = r"c:\Users\UvaanG\Desktop\enterprise\sa-funeral-api\sa-funerals-catalog\src\pages\catalog.component.ts"

with open(filepath, "r", encoding="utf-8") as f:
    content = f.read()

replacement_header = """<!-- Header & Search -->
        <div class="bg-gradient-to-r from-safs-primary to-[#2A3470] px-4 py-4 sm:px-8 sm:py-6 md:px-12 md:py-8 border-b border-white/10 shadow-lg relative z-20 flex-shrink-0 m-4 rounded-3xl">
          <div class="absolute inset-0 bg-radial-gradient from-safs-accent/10 via-transparent to-transparent pointer-events-none rounded-3xl"></div>
          <div class="max-w-6xl mx-auto flex flex-col md:flex-row gap-4 sm:gap-6 items-center justify-between relative z-10">
            <div class="text-center md:text-left">
              <h1 class="text-2xl sm:text-3xl lg:text-4xl font-bold text-white mb-2 tracking-wide font-sans">SAFS Catalog</h1>
              <p class="text-safs-accent text-xs sm:text-sm font-semibold tracking-[0.15em] uppercase">Premium range of caskets & professional funeral equipment</p>
            </div>
            
            <!-- Search Bar -->
            <div class="w-full md:w-[320px] relative flex-shrink-0">
              <div class="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none">
                <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="text-safs-primary">
                  <circle cx="11" cy="11" r="8"></circle>
                  <line x1="21" y1="21" x2="16.65" y2="16.65"></line>
                </svg>
              </div>
              <input 
                type="text" 
                placeholder="Search catalog..." 
                (input)="onSearch($event)"
                [value]="searchQuery()"
                class="w-full pl-10 pr-10 py-3 rounded-xl shadow-xl bg-white/95 border-2 border-transparent focus:border-safs-accent outline-none transition-all text-safs-primary text-sm placeholder-safs-primary/50 font-semibold" />
              @if (searchQuery()) {
                <button (click)="clearSearch()" class="absolute inset-y-0 right-0 pr-4 flex items-center text-safs-primary/50 hover:text-safs-primary transition-colors">
                  <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                    <line x1="18" y1="6" x2="6" y2="18"></line>
                    <line x1="6" y1="6" x2="18" y2="18"></line>
                  </svg>
                </button>
              }
            </div>
          </div>
        </div>
        """

pattern = r"<!--\s*Header\s*&\s*Search\s*-->.*?<!--\s*Product\s+Grid\s+Area\s*-->"

new_content, count = re.subn(pattern, replacement_header + "\n          <!-- Product Grid Area -->", content, flags=re.DOTALL)

if count > 0:
    with open(filepath, "w", encoding="utf-8") as f:
        f.write(new_content)
    print(f"SUCCESS: Replaced {count} occurrences")
else:
    print("PATTERN NOT MATCHED")
