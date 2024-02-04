<template>
    <dialog ref="dialog">
        <AppHeader
            v-if="header"
            :title="title"
        >
            <template #leftActions>
                <slot name="leftActions" />
            </template>
            <template #rightActions>
                <slot name="rightActions" />
            </template>
        </AppHeader>
        <slot name="header" />

        <main :class="{ padding: contentPadding }">
            <slot name="default" />
        </main>
    </dialog>
</template>

<script setup>
import { ref } from 'vue';
import AppHeader from '@/components/AppHeader.vue';

const dialog = ref(null);

defineProps({
    header: {
        type: Boolean,
        default: false,
    },
    title: {
        type: String,
        required: false,
    },
    contentPadding: {
        type: Boolean,
        default: true,
    },
});

const openDialog = () => {
    dialog.value.showModal();
};
const closeDialog = () => {
    dialog.value.close();
};

defineExpose({
    openDialog,
    closeDialog,
});
</script>

<style lang="scss" scoped>
dialog {
    background: var(--color-base-900);
    border: 0;
    color: var(--color-base-100);
    padding: 0;
    width: calc(100% - 50px);
    max-width: 550px;
    border-radius: 6px;
    box-shadow: 0px 6px 32px 4px rgba(0, 0, 0, 0.4);

    &::backdrop {
        background: rgba(0, 0, 0, 0.4);
    }

    & main {
        max-height: 70vh;
        overflow-y: auto;

        &.padding {
            padding: 20px 10px;
        }
    }
}
</style>
