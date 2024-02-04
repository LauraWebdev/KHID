<template>
    <AppHeader title="Download Queue">
        <template #leftActions>
            <button
                class="flat"
                @click="addDialog.dialog.openDialog()"
            >
                <span class="icon ri-add-line"></span>
            </button>
        </template>
        <template #rightActions>
            <button
                class="flat"
                @click="() => {}"
            >
                <span class="icon ri-settings-3-line"></span>
            </button>
            <button
                class="flat"
                @click="() => {}"
            >
                <span class="icon ri-information-2-fill"></span>
            </button>
        </template>
    </AppHeader>

    <main>
        <router-view></router-view>
    </main>

    <AddDialog ref="addDialog" />
</template>

<script setup>
import { inject, ref } from 'vue';
import AppHeader from '@/components/AppHeader.vue';
import AddDialog from '@/components/AddDialog.vue';

const addDialog = ref(null);

const emitter = inject('emitter');
window.external.receiveMessage((rawResponse) => {
    const response = JSON.parse(rawResponse);
    emitter.emit(response.Command, response.Data);
});
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
