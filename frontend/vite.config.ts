import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [
    react(),
  ],
  server: {
    proxy: {
      // Перенаправляємо всі запити, що починаються з /hubs, на наш C# API
      '/hubs': {
        target: 'http://localhost:5170', // Використовуємо HTTP порт вашого API
        ws: true, // Обов'язково вмикаємо підтримку WebSockets для SignalR!
        changeOrigin: true,
        secure: false,
      },
    },
  },
})