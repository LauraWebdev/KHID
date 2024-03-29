import {createRouter, createWebHashHistory} from 'vue-router';
import ViewIndex from '@/views/ViewIndex.vue';
import ViewSettings from '@/views/ViewSettings.vue';

const router = createRouter({
    history: createWebHashHistory(import.meta.env.BASE_URL),
    routes: [
        {
            path: '/',
            name: 'queue',
            component: ViewIndex,
        },
        {
            path: '/settings',
            name: 'settings',
            component: ViewSettings,
        },
    ],
});

export default router;
