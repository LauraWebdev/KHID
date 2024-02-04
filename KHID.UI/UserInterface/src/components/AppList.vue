<template>
    <div class="app-list">
        <slot />
    </div>
</template>

<script setup></script>

<style lang="scss">
.app-list {
    display: flex;
    flex-direction: column;
    justify-content: stretch;
    box-shadow: 0 1px 4px rgba(0, 0, 0, 0.2);
    overflow: hidden;
    border-radius: 6px;
    gap: 2px;

    & .app-list-row {
        background: var(--color-base-800);
        transition: 0.15s ease-in-out all;
        min-height: 55px;
        display: grid;
        grid-template-columns: 1fr auto;
        gap: 15px;
        align-items: center;
        padding: 0 15px;

        & .meta {
            display: flex;
            flex-direction: column;
            gap: 3px;

            & span:nth-child(1) {
            }
            & span:nth-child(2) {
                color: var(--color-base-500);
            }
        }
    }
}
</style>
