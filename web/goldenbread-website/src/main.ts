import './assets/index.css';
import '@/api/http/interceptors';
import { autoAnimatePlugin } from '@formkit/auto-animate/vue';
import { useLoginStore } from '@/features/login/loginStore';
import { createPinia } from 'pinia';
import { createApp } from 'vue';
import router from './utils/router';
import App from './App.vue';

const app = createApp(App);
// const loginStore = useLoginStore();

app.use(router);
app.use(autoAnimatePlugin);
app.use(createPinia());

// loginStore.checkSessionLocal();
// loginStore.checkSession();

app.mount('#app');
