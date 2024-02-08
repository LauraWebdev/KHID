<template>
    <Wrapper>
        <div class="settings">
            <AppList>
                <AppListActionRow
                    title="KHInsider Downloader"
                    subtitle="Version 1.0.0"
                >
                    <button>
                        <span>GitHub</span>
                    </button>
                </AppListActionRow>
            </AppList>

            <div v-if="!loading">
                <AppList>
                    <AppListActionRow
                        title="Default output path"
                        :subtitle="settingOutputDefaultPath ?? false"
                    >
                        <button
                            :disabled="saving"
                            @click="handleSelectOutputPath"
                        >
                            <span class="icon ri-folder-2-fill"></span>
                        </button>
                    </AppListActionRow>
                </AppList>
            </div>
            <div
                v-if="loading"
                class="loading"
            >
                <AppSpinner />
            </div>
        </div>
    </Wrapper>
</template>

<script setup>
import Wrapper from '@/components/Wrapper.vue';
import AppList from '@/components/AppList.vue';
import AppListActionRow from '@/components/AppListActionRow.vue';
import { inject, onMounted, onUnmounted, ref } from 'vue';
import AppSpinner from '@/components/AppSpinner.vue';

const loading = ref(false);
const saving = ref(false);
const emitter = inject('emitter');

const handleSelectOutputPath = () => {
    window.external.sendMessage(
        JSON.stringify({
            Command: 'output-path-select',
        }),
    );
};

const settingOutputDefaultPath = ref('');

onMounted(() => {
    loading.value = true;

    window.external.sendMessage(
        JSON.stringify({
            Command: 'settings-get-full',
        }),
    );

    emitter.on('output-path-select-response', (response) => {
        settingOutputDefaultPath.value = response;

        window.external.sendMessage(
            JSON.stringify({
                Command: 'settings-set',
                Data: [
                    {
                        key: 'output.defaultPath',
                        value: settingOutputDefaultPath.value,
                    },
                ],
            }),
        );
    });
    emitter.on('settings-get-full-response', (response) => {
        loading.value = false;

        settingOutputDefaultPath.value = response['output.defaultPath'] ?? '';
    });
});
onUnmounted(() => {
    emitter.off('output-path-select-response');
    emitter.off('settings-get-full-response');
});
</script>

<style lang="scss" scoped>
.settings {
    margin: 30px 0;
    display: grid;
    gap: 15px;
}
.loading {
    padding: 30px 0;
    display: flex;
    justify-content: center;
    align-items: center;
}
</style>
