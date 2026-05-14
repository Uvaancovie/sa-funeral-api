export default function imageLoader({ src, width, quality }) {
  // For Supabase images, append transform params
  if (src.includes('supabase.co')) {
    const url = new URL(src);
    url.searchParams.set('width', width);
    url.searchParams.set('height', width); // Assuming square, adjust as needed
    url.searchParams.set('quality', quality || 80);
    url.searchParams.set('resize', 'cover');
    return url.toString();
  }
  // Fallback to default
  return `${src}?w=${width}&q=${quality || 75}`;
}