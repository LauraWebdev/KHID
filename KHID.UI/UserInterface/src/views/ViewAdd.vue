<template>
    <div v-if="step === 1 && !isLoading">
        Step 1 = Enter URL<br />
        <input v-model="urlOrSlug" />
        <button @click="getSoundtrackInfo">Continue</button>
    </div>
    <div v-if="step === 2 && !isLoading">
        Step 2 = Output Folder, Formats, Select Songs<br />
    </div>
    <div v-if="isLoading">
        Loading...
    </div>
</template>

<script setup>
import { inject, onMounted, onUnmounted, ref } from 'vue';
const emitter = inject('emitter');

const step = ref(1);
const isLoading = ref(false);
const urlOrSlug = ref('');

const soundtrack = ref(null);

const getSoundtrackInfo = () => {
    isLoading.value = true;

    window.external.sendMessage(
        JSON.stringify({
            Command: 'soundtrack-get',
            Data: 'portal',
        }),
    );
};

onMounted(() => {
    emitter.on('soundtrack-get-response', (response) => {
        soundtrack.value = response;
        isLoading.value = false;
        step.value = 2;
    });
});
onUnmounted(() => {
    emitter.off('soundtrack-get-response');
});
</script>

<style lang="scss" scoped></style>
