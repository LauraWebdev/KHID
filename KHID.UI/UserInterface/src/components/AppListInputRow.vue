<template>
    <label
        class="app-list-row input-row"
        :class="{ disabled: disabled }"
    >
        <div class="meta">
            <span :class="{ 'has-value': val.length > 0 }">{{ title }}</span>
        </div>
        <input
            type="text"
            v-model="val"
            :disabled="disabled"
            @input="handleInput"
        />
        <div class="actions">
            <button class="flat">
                <span class="icon ri-pencil-fill"></span>
            </button>
        </div>
    </label>
</template>

<script setup>
import { ref, watch } from 'vue';

const props = defineProps({
    title: {
        type: String,
        required: true,
    },
    modelValue: {
        type: String,
        default: '',
    },
    disabled: {
        type: Boolean,
        default: false,
    },
});

const emit = defineEmits(['update:modelValue']);

const val = ref('');

watch(
    () => props.modelValue,
    (nVal) => (val.value = nVal),
    {
        immediate: true,
    },
);

const handleInput = () => emit('update:modelValue', val.value);
</script>

<style lang="scss">
.app-list-row {
    position: relative;
    z-index: 2;
    cursor: text;

    &.input-row {
        &:not(.disabled):hover {
            background: var(--color-base-700);
        }
        &.disabled {
            opacity: 0.6;
        }
        input {
            position: absolute;
            top: 22px;
            bottom: 2px;
            left: 15px;
            right: 15px;
            z-index: 1;
            font-family: var(--font-family);
            font-size: 1rem;
            color: var(--color-base-300);
            background: transparent;
            border: 0;

            &:focus {
                outline: none;
            }
        }

        & .meta {
            position: relative;

            & span {
                &:nth-child(1) {
                    position: absolute;
                    transition: 0.15s ease-in-out all;
                    top: -7px;

                    &.has-value {
                        top: -18px;
                    }
                }
            }
        }
        & .actions {
            transition: 0.15s ease-in-out all;
        }

        &:has(input:focus) {
            & .meta {
                & span {
                    &:nth-child(1) {
                        top: -18px;
                    }
                }
            }
            & .actions {
                opacity: 0;
            }
        }
    }
}
</style>
