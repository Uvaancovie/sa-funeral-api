/** @type {import('next').NextConfig} */
const nextConfig = {
  images: {
    domains: ['hcestxaffzsqlkiedvfx.supabase.co'],
    // Optionally, use a custom loader for Supabase transforms
    loader: 'custom',
    loaderFile: './lib/image-loader.js',
  },
};

module.exports = nextConfig;