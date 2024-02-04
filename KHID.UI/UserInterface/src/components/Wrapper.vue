<template>
    <div class="wrapper">
        <slot />
    </div>
</template>

<script setup></script>

<style lang="scss" scoped>
.wrapper {
    width: calc(100% - 20px);
    max-width: 650px;
    margin: 0 auto;
    display: grid;
    align-items: flex-start;
}
</style>
