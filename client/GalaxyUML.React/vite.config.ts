import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import tailwindcss from '@tailwindcss/vite';

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    react(),
    tailwindcss(),
  ],
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'http://localhost:5248',
        changeOrigin: true,
        secure: false,
      },
      '/diagramHub': {
        target: 'http://localhost:5248',
        ws: true,
        changeOrigin: true,
        secure: false,
      },
    },
  },
});
