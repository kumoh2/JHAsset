import { createApp } from 'vue';
import App from './App.vue';
import router from './router';      // ← 꼭 import

createApp(App).use(router).mount('#app');
