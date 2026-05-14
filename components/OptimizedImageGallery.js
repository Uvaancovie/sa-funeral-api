import { createClient } from '@supabase/supabase-js';
import Image from 'next/image';

const supabaseUrl = process.env.NEXT_PUBLIC_SUPABASE_URL;
const supabaseKey = process.env.NEXT_PUBLIC_SUPABASE_ANON_KEY;
const supabase = createClient(supabaseUrl, supabaseKey);

export default async function OptimizedImageGallery() {
  // Fetch image paths from database or storage
  // Assuming images are stored in 'images' bucket
  const { data: files } = await supabase.storage.from('images').list();

  const images = files?.map(file => {
    // Get optimized public URL
    const { data } = supabase.storage.from('images').getPublicUrl(file.name, {
      transform: {
        width: 800,
        height: 600,
        quality: 80,
        resize: 'cover',
      },
    });
    return data.publicUrl;
  }) || [];

  return (
    <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
      {images.map((url, index) => (
        <div key={index} className="relative w-full h-64">
          <Image
            src={url}
            alt={`Image ${index + 1}`}
            fill
            sizes="(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 33vw"
            priority={index === 0} // Priority for LCP
            loading={index === 0 ? 'eager' : 'lazy'}
            className="object-cover rounded-lg"
          />
        </div>
      ))}
    </div>
  );
}