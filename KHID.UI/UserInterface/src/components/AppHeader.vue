<template>
    <header>
        <nav>
            <slot name="leftActions" />
        </nav>
        <div class="title">{{ title }}</div>
        <nav>
            <slot name="rightActions" />
        </nav>
    </header>
</template>

<script setup>
defineProps({
    title: {
        type: String,
        required: true,
    },
});
</script>

<style lang="scss" scoped>
header {
    position: relative;
    z-index: 10;
    display: grid;
    grid-template-columns: 1fr auto 1fr;
    align-items: center;
    gap: 15px;
    background: var(--color-base-800);
    padding: 8px;
    box-shadow: 0px -1px 6px 1px var(--color-base-950);
    user-select: none;
    -webkit-user-select: none;
    cursor: default;
    height: 50px;

    & nav {
        display: flex;
        gap: 5px;
        align-items: center;

        &:nth-child(3) {
            justify-content: flex-end;
        }
    }
}
</style>
