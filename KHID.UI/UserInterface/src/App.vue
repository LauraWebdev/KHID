<template>
    <header>Header</header>

    <router-view></router-view>
</template>

<script setup>
import { inject } from 'vue';

const emitter = inject('emitter');
window.external.receiveMessage((rawResponse) => {
    const response = JSON.parse(rawResponse);
    emitter.emit(response.Command, response.Data);
});
</script>

<style lang="scss" scoped></style>
