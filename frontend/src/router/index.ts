import { createRouter, createWebHistory } from 'vue-router';
import AssetManager from '@/components/AssetManager.vue';

export default createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', component: AssetManager },       // 기본 화면
  ],
});
