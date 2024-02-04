<template>
    <div>Index</div>
    <button @click="test">Test</button>
</template>

<script setup>
import { inject, onMounted, onUnmounted } from 'vue';

const emitter = inject('emitter');

const test = () => {
    window.external.sendMessage(
        JSON.stringify({
            Command: 'soundtrack-get',
            Data: 'portal',
        }),
    );
};

onMounted(() => {
    emitter.on('soundtrack-get-response', (response) => {
        console.log(response);
    });
});
onUnmounted(() => {
    emitter.off('soundtrack-get-response');
});
</script>

<style lang="scss" scoped></style>
