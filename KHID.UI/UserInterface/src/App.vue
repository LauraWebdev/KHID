<template>
    <AppHeader :title="appHeaderTitle">
        <template #leftActions>
            <button
                v-if="route.name === 'queue'"
                class="flat"
                @click="addDialog.dialog.openDialog()"
            >
                <span class="icon ri-add-line"></span>
            </button>
            <button
                v-if="route.name !== 'queue'"
                class="flat"
                @click="handleBack"
            >
                <span class="icon ri-arrow-left-line"></span>
            </button>
        </template>
        <template #rightActions>
            <button
                class="flat"
                @click="handleSettings"
            >
                <span class="icon ri-settings-3-line"></span>
            </button>
        </template>
    </AppHeader>

    <main>
        <router-view></router-view>
    </main>

    <AddDialog ref="addDialog" />
</template>

<script setup>
import { computed, inject, ref } from 'vue';
import AppHeader from '@/components/AppHeader.vue';
import AddDialog from '@/components/AddDialog.vue';
import router from '@/router';
import { useRoute } from 'vue-router';

const route = useRoute();
const appHeaderTitle = computed(() => {
    switch (route.name) {
        default:
            return 'KHInsider Downloader';
        case 'queue':
            return 'Download Queue';
        case 'settings':
            return 'Settings';
    }
});

const addDialog = ref(null);

const emitter = inject('emitter');
window.external.receiveMessage((rawResponse) => {
    const response = JSON.parse(rawResponse);
    emitter.emit(response.Command, response.Data);
});

const handleSettings = () => {
    router.push({ name: 'settings' });
};
const handleBack = () => {
    router.push({ name: 'queue' });
};
const handleAbout = () => {
    router.push({ name: 'about' });
};
</script>

<style lang="scss">
#app {
    position: absolute;
    inset: 0;
    overflow: hidden;
    display: grid;
    grid-template-rows: auto 1fr;

    & > main {
        overflow-y: auto;
    }
}
</style>
