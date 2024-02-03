import { createApp } from 'vue';
import App from './App.vue';

import './assets/reset.scss';
import '@fontsource-variable/manrope';
import 'remixicon/fonts/remixicon.css';
import './assets/colors.scss';
import './assets/app.scss';
import mitt from 'mitt';
import router from '@/router';

const app = createApp(App);

app.provide('emitter', new mitt());
app.use(router);

app.mount('#app');
