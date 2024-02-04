<template>
    <div class="app-list-row action-row">
        <div class="meta">
            <span>{{ title }}</span>
            <span v-if="subtitle">{{ subtitle }}</span>
        </div>
        <div class="action">
            <slot />
        </div>
    </div>
</template>

<script setup>
defineProps({
    title: {
        type: String,
        required: true,
    },
    subtitle: {
        type: [String, Boolean],
        default: false,
    },
});
</script>

<style lang="scss" scoped>
.action-row {
    cursor: default;
}
</style>
