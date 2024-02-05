<template>
    <div
        class="app-list-row queue-item-row"
        :class="`${getStateClass(queueItem.State)} ${getResultClass(queueItem.Result)}`"
    >
        <div class="icon">
            <span
                class="ri-time-line"
                v-if="getStateClass(queueItem.State) === 'state-queued'"
            ></span>
            <span
                class="ri-check-line"
                v-if="getStateClass(queueItem.State) === 'state-done' && getResultClass(queueItem.Result) === 'result-succeeded'"
            ></span>
            <span
                class="ri-close-line"
                v-if="getStateClass(queueItem.State) === 'state-done' && getResultClass(queueItem.Result) === 'result-error'"
            ></span>
            <AppSpinner v-if="getStateClass(queueItem.State) === 'state-downloading'" />
        </div>
        <div class="meta">
            <span>{{ queueItem.Title }}</span>
            <span>{{ queueItem.SoundtrackTitle }}</span>
        </div>
        <div
            class="action"
            v-show="getStateClass(queueItem.State) === 'state-downloading'"
        >
            <div class="progress-bar">
                <div
                    class="value"
                    :style="`width: ${itemProgress}%`"
                ></div>
            </div>
            <span>{{ `${itemProgress.toFixed(1)}%` }}</span>
        </div>
    </div>
</template>

<script setup>
import AppSpinner from '@/components/AppSpinner.vue';
import { computed } from 'vue';

const props = defineProps({
    queueItem: {
        type: Object,
        required: true,
    },
});

const itemProgress = computed(() => {
    return Math.round(props.queueItem.DownloadProgress * 10) / 10;
});

const getStateClass = (state) => {
    switch (state) {
        default:
            return 'state-default';
        case 0:
            return 'state-queued';
        case 1:
            return 'state-downloading';
        case 2:
            return 'state-done';
    }
};

const getResultClass = (result) => {
    switch (result) {
        default:
            return 'result-unknown';
        case 0:
            return 'result-error';
        case 1:
            return 'result-succeeded';
    }
};
</script>

<style lang="scss" scoped>
.queue-item-row {
    cursor: default;
    grid-template-columns: 32px 1fr auto;
    user-select: none;
    -webkit-user-select: none;

    & .icon {
        width: 32px;
        height: 32px;
        display: flex;
        justify-content: center;
        align-items: center;
        font-size: 24px;
        opacity: 0.6;
    }

    & .action {
        display: flex;
        gap: 5px;
        flex-direction: column;
        align-items: center;

        & .progress-bar {
            margin-top: 2px;
            width: 80px;
            height: 4px;
            background: var(--color-base-300);
            border-radius: 5px;
            overflow: hidden;

            & .value {
                height: 4px;
                background: var(--color-base-500);
                transition: 0.2s ease-in-out width;
            }
        }

        & span {
            font-size: 0.75rem;
            font-family: monospace;
        }
    }

    &.state-downloading {
        background: var(--color-base-200);
        color: var(--color-base-700);

        & .meta {
            & span:nth-child(2) {
                color: var(--color-base-400);
            }
        }
    }

    &.state-done.result-error {
        background: rgba(var(--color-danger-500), 0.4);
        color: rgb(var(--color-danger-300));

        & .meta {
            & span:nth-child(2) {
                color: rgba(var(--color-danger-400), 0.6);
            }
        }
    }
    &.state-queued {
        opacity: 0.4;
    }
}
</style>
