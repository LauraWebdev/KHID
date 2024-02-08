<template>
    <AppDialog
        ref="dialog"
        :header="true"
        title="Add a soundtrack"
    >
        <template #leftActions>
            <button
                @click="dialog.closeDialog()"
                :disabled="loading"
                v-if="step === 1"
            >
                <span>Cancel</span>
            </button>
            <button
                @click="step = 1"
                v-if="step === 2"
                :disabled="addingToQueue"
            >
                <span>Back</span>
            </button>
        </template>
        <template #rightActions>
            <button
                class="primary"
                :disabled="loading || albumUrlOrSlug.length < 3"
                @click="handleLoadAlbum"
                v-if="step === 1"
            >
                <span>Continue</span>
            </button>
            <button
                class="primary"
                :disabled="addingToQueue || outputPath === '' || folderName === ''"
                @click="handleAddToQueue"
                v-if="step === 2"
            >
                <span>Add to queue</span>
            </button>
        </template>
        <template #default>
            <div
                v-if="!loading && step === 1"
                class="content"
            >
                <AppList>
                    <AppListInputRow
                        title="Album"
                        v-model="albumUrlOrSlug"
                        :disabled="loading"
                    />
                </AppList>

                <div class="explainer">
                    <span>Examples:</span>
                    <span>- https://downloads.khinsider.com/game-soundtracks/album/portal</span>
                    <span>- downloads.khinsider.com/game-soundtracks/album/portal</span>
                    <span>- portal</span>
                </div>
            </div>
            <div
                v-if="!loading && step === 2"
                class="content"
            >
                <AppList>
                    <AppListActionRow
                        title="Output path"
                        :subtitle="outputPath ?? false"
                    >
                        <button
                            :disabled="addingToQueue"
                            @click="handleSelectOutputPath"
                        >
                            <span class="icon ri-folder-2-fill"></span>
                        </button>
                    </AppListActionRow>
                    <AppListInputRow
                        title="Folder name"
                        v-model="folderName"
                        :disabled="addingToQueue"
                    />
                    <AppListActionRow title="Format">
                        <div class="combo-buttons">
                            <button
                                v-for="item in album?.Formats"
                                :key="item"
                                :class="{ primary: format === item }"
                                @click="format = item"
                                :disabled="addingToQueue"
                            >
                                <span>.{{ item }}</span>
                            </button>
                        </div>
                    </AppListActionRow>
                </AppList>

                <AppList>
                    <AppListActionRow
                        v-for="song in album?.Songs"
                        :key="song.Url"
                        :title="song.Title"
                    >
                        <button
                            v-if="includedSongs.includes(song)"
                            class="primary"
                            @click="includedSongs = includedSongs.filter((x) => x !== song)"
                        >
                            <span class="icon ri-check-line"></span>
                        </button>
                        <button
                            v-if="!includedSongs.includes(song)"
                            @click="includedSongs.push(song)"
                        >
                            <span class="icon ri-close-line"></span>
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
        </template>
    </AppDialog>
</template>

<script setup>
import { inject, onMounted, onUnmounted, ref } from 'vue';
import AppDialog from '@/components/AppDialog.vue';
import AppList from '@/components/AppList.vue';
import AppListInputRow from '@/components/AppListInputRow.vue';
import AppSpinner from '@/components/AppSpinner.vue';
import AppListActionRow from '@/components/AppListActionRow.vue';

const emitter = inject('emitter');

const dialog = ref(null);
const loading = ref(false);
const albumUrlOrSlug = ref('');
const addingToQueue = ref(false);
const outputPath = ref('');
const folderName = ref('');
const format = ref('');

const album = ref(null);
const step = ref(1);
const includedSongs = ref([]);

const handleLoadAlbum = () => {
    loading.value = true;

    window.external.sendMessage(
        JSON.stringify({
            Command: 'soundtrack-get',
            Data: albumUrlOrSlug.value,
        }),
    );
};

const handleSelectOutputPath = () => {
    window.external.sendMessage(
        JSON.stringify({
            Command: 'output-path-select',
        }),
    );
};

const handleAddToQueue = () => {
    loading.value = true;
    addingToQueue.value = true;

    let songs = [];
    includedSongs.value.forEach((song) => {
        songs.push({
            ...song,
            OutputPath: outputPath.value + '/' + folderName.value,
            SoundtrackTitle: album.value.Title,
            Format: format.value,
        });
    });

    window.external.sendMessage(
        JSON.stringify({
            Command: 'queue-add',
            Data: songs,
        }),
    );

    loading.value = true;
};

onMounted(() => {
    window.external.sendMessage(
        JSON.stringify({
            Command: 'settings-get',
            Data: 'output.defaultPath',
        }),
    );

    emitter.on('soundtrack-get-response', (response) => {
        album.value = response;
        folderName.value = album.value.Slug;
        format.value = album.value.Formats[0];

        album.value.Songs?.forEach((song) => {
            includedSongs.value.push(song);
        });

        loading.value = false;
        step.value = 2;
    });
    emitter.on('output-path-select-response', (response) => {
        outputPath.value = response;
    });
    emitter.on('settings-get-response', (response) => {
        if (response.key === 'output.defaultPath') {
            outputPath.value = response.data;
        }
    });
    emitter.on('queue-add-response', (response) => {
        loading.value = false;
        addingToQueue.value = false;
        step.value = 1;
        album.value = null;
        folderName.value = '';
        format.value = '';
        albumUrlOrSlug.value = '';
        includedSongs.value = [];
        dialog.value.closeDialog();
    });
});
onUnmounted(() => {
    emitter.off('soundtrack-get-response');
    emitter.off('output-path-select-response');
    emitter.off('settings-get-response');
    emitter.off('queue-add-response');
});

defineExpose({
    dialog,
});
</script>

<style lang="scss" scoped>
.content {
    display: grid;
    gap: 15px;

    & .explainer {
        display: flex;
        flex-direction: column;
        gap: 10px;
        color: var(--color-base-500);
        font-size: 0.85rem;
    }
    & .combo-buttons {
        display: flex;
        gap: 2px;
        border-radius: 6px;
        overflow: hidden;

        & button {
            border-radius: 0;
        }
    }
}
.loading {
    padding: 30px 0;
    display: flex;
    justify-content: center;
    align-items: center;
}
</style>
