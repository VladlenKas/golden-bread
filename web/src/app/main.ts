import '@/app/styles/index.css';
import { autoAnimatePlugin } from '@formkit/auto-animate/vue';
import { createApp } from 'vue';
import { router } from './providers/router';
import { pinia } from './providers/store';
import App from './ui/App.vue';

const app = createApp(App);

app.use(pinia);
app.use(router);
app.use(autoAnimatePlugin);

app.mount('#app');
