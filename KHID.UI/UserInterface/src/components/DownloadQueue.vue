<template>
    <Wrapper>
        <div class="download-queue">
            <div
                class="queue-empty"
                v-if="queue.length === 0"
            >
                <span class="icon ri-file-download-line"></span>
                <h1>Queue is empty</h1>
                <p>Add media to the queue to start downloading</p>
            </div>
            <AppList v-if="queue.length > 0">
                <AppListQueueItemRow
                    v-for="item in queue"
                    :key="item.Url"
                    :queue-item="item"
                />
            </AppList>
        </div>
    </Wrapper>
</template>

<script setup>
import Wrapper from '@/components/Wrapper.vue';
import { inject, onMounted, onUnmounted, ref } from 'vue';
import AppList from '@/components/AppList.vue';
import AppListQueueItemRow from '@/components/AppListQueueItemRow.vue';

const emitter = inject('emitter');
const queue = ref([]);

onMounted(() => {
    emitter.on('queue-get-response', (response) => {
        queue.value = response;
    });
    emitter.on('queue-updated-response', (response) => {
        queue.value = response;
    });
    emitter.on('queue-item-progress-response', (response) => {
        let index = queue.value.findIndex((x) => x.Url === response.Url);

        if (index !== -1) {
            queue.value[index] = response;
        } else {
            queue.value.push(response);
        }
    });

    window.external.sendMessage(
        JSON.stringify({
            Command: 'queue-get',
        }),
    );
});
onUnmounted(() => {
    emitter.off('queue-get-response');
    emitter.off('queue-updated-response');
    emitter.off('queue-item-progress-response');
});
</script>

<style lang="scss" scoped>
.download-queue {
    margin: 30px 0;

    & .queue-empty {
        padding: 50px 0;
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        gap: 10px;

        & .icon {
            width: 48px;
            height: 48px;
            display: flex;
            justify-content: center;
            align-items: center;
            font-size: 40px;
            color: var(--color-base-500);
            margin-bottom: 15px;
        }
        & h1 {
            font-size: 1.75rem;
        }
        & p {
            color: var(--color-base-500);
            line-height: 1.5rem;
            text-align: center;
        }
    }
}
</style>
