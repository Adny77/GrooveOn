import 'package:flutter/material.dart';
import 'package:grooveon_desktop/deezer/helper/deezer_search_helper.dart';
import 'package:grooveon_desktop/deezer/models/deezer_album.dart';
import 'package:grooveon_desktop/deezer/models/deezer_track.dart';
import 'package:grooveon_desktop/dialogs/base_dialogs_frame.dart';
import 'package:grooveon_desktop/dialogs/confirmation_dialogs.dart';
import 'package:grooveon_desktop/models/request/album_upsert_request.dart';
import 'package:grooveon_desktop/models/request/song_bulk_insert_request.dart';
import 'package:grooveon_desktop/models/request/song_duplicate_check_request.dart';
import 'package:grooveon_desktop/models/request/song_upsert_request.dart';
import 'package:grooveon_desktop/models/response/album_preview_response.dart';
import 'package:grooveon_desktop/models/response/album_save_response.dart';
import 'package:grooveon_desktop/models/response/song_bulk_insert_response.dart';
import 'package:grooveon_desktop/models/response/song_duplicate_check_response.dart';
import 'package:grooveon_desktop/providers/album_provider.dart';
import 'package:grooveon_desktop/providers/song_provider.dart';


import 'package:grooveon_desktop/screens/music_screen.dart';

enum AddMusicMode {
  song,
  album,
}

class MusicAddContent extends StatefulWidget {
  const MusicAddContent({super.key});

  @override
  State<MusicAddContent> createState() => _MusicAddContentState();
}

class _MusicAddContentState extends State<MusicAddContent> {
  AddMusicMode _mode = AddMusicMode.song;

  late final DeezerMusicHelper _deezerHelper;
  final SongProvider _songProvider = SongProvider();

  final AlbumProvider _albumProvider = AlbumProvider();

  final TextEditingController _songSearchController = TextEditingController();
  final TextEditingController _albumSearchController = TextEditingController();

  final List<_SongPreviewModel> _selectedSongs = [];

  bool _isSavingSongs = false;
  bool _isSavingAlbum = false;

  @override
  void initState() {
    super.initState();
    _deezerHelper = DeezerMusicHelper();
    _deezerHelper.addListener(_onHelperChanged);
    _deezerHelper.loadInitialTopTracks();
  }

  void _onHelperChanged() {
    if (mounted) {
      setState(() {});
    }
  }

  @override
  void dispose() {
    _deezerHelper.removeListener(_onHelperChanged);
    _deezerHelper.dispose();
    _songSearchController.dispose();
    _albumSearchController.dispose();
    super.dispose();
  }

  Future<void> _searchSongs() async {
    await _deezerHelper.searchSongs(_songSearchController.text);
  }

  Future<void> _searchAlbums() async {
    await _deezerHelper.searchAlbums(_albumSearchController.text);
  }

  void _addSongToSelection(DeezerTrack track) {
    final exists = _selectedSongs.any((x) => x.id == track.id);

    if (exists) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text("Song is already added to the selected list."),
        ),
      );
      return;
    }

    setState(() {
      _selectedSongs.add(
        _SongPreviewModel(
          id: track.id,
          externalTrackId: track.id.toString(),
          title: track.title,
          artist: track.artist?.name ?? 'Unknown artist',
          duration: _formatDuration(track.duration),
          durationSeconds: track.duration ?? 0,
          coverUrl: track.album?.coverMedium ?? track.album?.coverBig,
          previewUrl: track.preview,
          albumTitle: track.album?.title,
        ),
      );
    });
  }

  void _removeSongFromSelection(int id) {
    setState(() {
      _selectedSongs.removeWhere((x) => x.id == id);
    });
  }

  void _clearSelectedSongs() {
    setState(() {
      _selectedSongs.clear();
    });
  }

  SongUpsertRequest _mapToSongUpsertRequest(_SongPreviewModel song) {
    return SongUpsertRequest(
      externalTrackId: song.externalTrackId,
      source: "Deezer",
      title: song.title,
      artistName: song.artist,
      albumTitle: song.albumTitle,
      durationSeconds: song.durationSeconds,
      previewUrl: song.previewUrl,
      coverUrl: song.coverUrl,
      releaseDate: null,
    );
  }

  AlbumUpsertRequest _mapAlbumDetailsToRequest(dynamic details) {
    return AlbumUpsertRequest(
      externalAlbumId: details.id.toString(),
      externalArtistId: details.artist?.id.toString(),
      source: "Deezer",
      title: details.title,
      artistName: details.artist?.name ?? 'Unknown artist',
      coverUrl: details.coverMedium ?? details.coverBig ?? details.cover,
      description: null,
      releaseDate: details.releaseDate != null
          ? DateTime.tryParse(details.releaseDate!)
          : null,
      tracks: details.tracks.data.map<SongUpsertRequest>((track) {
        return SongUpsertRequest(
          externalTrackId: track.id.toString(),
          source: "Deezer",
          title: track.title,
          artistName: track.artist?.name ?? details.artist?.name ?? 'Unknown artist',
          albumTitle: details.title,
          durationSeconds: track.duration ?? 0,
          previewUrl: track.preview,
          coverUrl: track.album?.coverMedium ??
              track.album?.coverBig ??
              details.coverMedium ??
              details.coverBig,
          releaseDate: details.releaseDate != null
              ? DateTime.tryParse(details.releaseDate!)
              : null,
        );
      }).toList(),
    );
  }

  Future<void> _saveSongs() async {
    if (_selectedSongs.isEmpty || _isSavingSongs) return;

    setState(() {
      _isSavingSongs = true;
    });

    try {
      final duplicateResponse = await _songProvider.checkDuplicates(
        SongDuplicateCheckRequest(
          externalTrackIds: _selectedSongs.map((e) => e.externalTrackId).toList(),
        ),
      );

      if (!mounted) return;

      if (duplicateResponse.existingSongs.isNotEmpty) {
        setState(() {
          _isSavingSongs = false;
        });

        _showExistingSongsDialog(duplicateResponse);
        return;
      }

      final confirmed = await ConfirmDialogs.yesNoConfirmation(
        context,
        title: "Confirmation",
        question: "Are you sure you want to save all selected songs?",
      );

      if (confirmed != true) {
        setState(() {
          _isSavingSongs = false;
        });
        return;
      }

      final request = SongBulkInsertRequest(
        songs: _selectedSongs.map(_mapToSongUpsertRequest).toList(),
      );

      final SongBulkInsertResponse result =
          await _songProvider.bulkSaveDeezerSongs(request);

      if (!mounted) return;

      setState(() {
        _isSavingSongs = false;
        _selectedSongs.clear();
      });

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            "${result.savedCount} song(s) successfully saved.",
          ),
        ),
      );
    } catch (e) {
      if (!mounted) return;

      setState(() {
        _isSavingSongs = false;
      });

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text("Error while saving songs: $e"),
        ),
      );
    }
  }

  void _showExistingSongsDialog(SongDuplicateCheckResponse response) {
    final existingSongs = response.existingSongs;
    final missingIds = response.missingExternalTrackIds;

    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (_) => BaseDialog(
        title: "Some songs already exist",
        width: 760,
        height: 560,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              existingSongs.length == _selectedSongs.length
                  ? "All selected songs are already saved in the database."
                  : "Some selected songs are already saved in the database. Review them below.",
              style: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.w600,
                color: MusicScreen.textColor,
              ),
            ),
            const SizedBox(height: 16),
            Expanded(
              child: existingSongs.isEmpty
                  ? const _EmptyState(
                      title: "No existing songs",
                      subtitle: "All selected songs are new.",
                    )
                  : ListView.separated(
                      itemCount: existingSongs.length,
                      separatorBuilder: (_, __) => const SizedBox(height: 10),
                      itemBuilder: (context, index) {
                        final song = existingSongs[index];
                        return _ExistingSongTile(song: song);
                      },
                    ),
            ),
            const SizedBox(height: 18),
            Row(
              children: [
                Expanded(
                  child: OutlinedButton(
                    onPressed: () => Navigator.of(context).pop(),
                    style: OutlinedButton.styleFrom(
                      minimumSize: const Size.fromHeight(46),
                      side: const BorderSide(color: MusicScreen.borderColor),
                    ),
                    child: const Text("Close"),
                  ),
                ),
                if (missingIds.isNotEmpty) ...[
                  const SizedBox(width: 12),
                  Expanded(
                    child: ElevatedButton(
                      onPressed: () async {
                        Navigator.of(context).pop();
                        await _saveOnlyMissingSongs(missingIds);
                      },
                      style: ElevatedButton.styleFrom(
                        minimumSize: const Size.fromHeight(46),
                        backgroundColor: MusicScreen.primaryColor,
                        foregroundColor: Colors.white,
                        elevation: 0,
                      ),
                      child: const Text("Continue with new songs"),
                    ),
                  ),
                ],
              ],
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _saveOnlyMissingSongs(List<String> missingExternalTrackIds) async {
    final songsToSave = _selectedSongs
        .where((x) => missingExternalTrackIds.contains(x.externalTrackId))
        .toList();

    if (songsToSave.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text("There are no new songs to save."),
        ),
      );
      return;
    }

    final confirmed = await ConfirmDialogs.yesNoConfirmation(
      context,
      title: "Confirmation",
      question:
          "Some songs already exist. Do you want to save only the new songs?",
    );

    if (confirmed != true) return;

    setState(() {
      _isSavingSongs = true;
    });

    try {
      final request = SongBulkInsertRequest(
        songs: songsToSave.map(_mapToSongUpsertRequest).toList(),
      );

      final SongBulkInsertResponse result =
          await _songProvider.bulkSaveDeezerSongs(request);

      if (!mounted) return;

      setState(() {
        _isSavingSongs = false;
        _selectedSongs.removeWhere(
          (x) => missingExternalTrackIds.contains(x.externalTrackId),
        );
      });

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            "${result.savedCount} new song(s) successfully saved.",
          ),
        ),
      );
    } catch (e) {
      if (!mounted) return;

      setState(() {
        _isSavingSongs = false;
      });

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text("Error while saving new songs: $e"),
        ),
      );
    }
  }

  Future<void> _openAlbumPreview(DeezerAlbum album) async {
    try {
      final details = await _deezerHelper.getAlbumDetails(album.id);

      if (!mounted) return;

      if (details == null) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(
              _deezerHelper.albumDetailsError ??
                  'Error while loading album details.',
            ),
          ),
        );
        return;
      }

      final request = _mapAlbumDetailsToRequest(details);
      final preview = await _albumProvider.previewDeezerAlbum(request);

      if (!mounted) return;

      _showAlbumPreviewDialog(request, preview);
    } catch (e) {
      if (!mounted) return;

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text("Error while previewing album: $e"),
        ),
      );
    }
  }

  void _showAlbumPreviewDialog(
    AlbumUpsertRequest request,
    AlbumPreviewResponse preview,
  ) {
    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (_) => BaseDialog(
        title: "Album preview",
        width: 760,
        height: 620,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              preview.albumAlreadyExists
                  ? "Album already exists in the database."
                  : "Album is new and can be added.",
              style: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.w600,
                color: MusicScreen.textColor,
              ),
            ),
            const SizedBox(height: 10),
            Text(
              "${request.title} • ${request.artistName}",
              style: const TextStyle(
                fontSize: 15,
                fontWeight: FontWeight.w700,
                color: MusicScreen.textColor,
              ),
            ),
            const SizedBox(height: 16),
            Expanded(
              child: ListView.separated(
                itemCount: preview.tracks.length,
                separatorBuilder: (_, __) => const SizedBox(height: 10),
                itemBuilder: (context, index) {
                  final track = preview.tracks[index];
                  return Container(
                    padding: const EdgeInsets.all(13),
                    decoration: BoxDecoration(
                      color: const Color(0xFFFCFCFD),
                      borderRadius: BorderRadius.circular(12),
                      border: Border.all(color: MusicScreen.borderColor),
                    ),
                    child: Row(
                      children: [
                        Expanded(
                          child: Text(
                            track.title,
                            style: const TextStyle(
                              fontSize: 13,
                              fontWeight: FontWeight.w700,
                              color: MusicScreen.textColor,
                            ),
                          ),
                        ),
                        Container(
                          padding: const EdgeInsets.symmetric(
                            horizontal: 10,
                            vertical: 6,
                          ),
                          decoration: BoxDecoration(
                            color: track.alreadyExists
                                ? const Color(0xFFFFF0F0)
                                : const Color(0xFFEFFAF1),
                            borderRadius: BorderRadius.circular(999),
                          ),
                          child: Text(
                            track.alreadyExists ? "Already exists" : "Will be added",
                            style: TextStyle(
                              fontSize: 11,
                              fontWeight: FontWeight.w700,
                              color: track.alreadyExists
                                  ? const Color(0xFFC62828)
                                  : const Color(0xFF2E7D32),
                            ),
                          ),
                        ),
                      ],
                    ),
                  );
                },
              ),
            ),
            const SizedBox(height: 16),
            Row(
              children: [
                Expanded(
                  child: OutlinedButton(
                    onPressed: () => Navigator.of(context).pop(),
                    style: OutlinedButton.styleFrom(
                      minimumSize: const Size.fromHeight(46),
                      side: const BorderSide(color: MusicScreen.borderColor),
                    ),
                    child: const Text("Cancel"),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: ElevatedButton(
                    onPressed: _isSavingAlbum
                        ? null
                        : () async {
                            Navigator.of(context).pop();
                            await _saveAlbum(request);
                          },
                    style: ElevatedButton.styleFrom(
                      minimumSize: const Size.fromHeight(46),
                      backgroundColor: MusicScreen.primaryColor,
                      foregroundColor: Colors.white,
                      elevation: 0,
                    ),
                    child: _isSavingAlbum
                        ? const SizedBox(
                            width: 18,
                            height: 18,
                            child: CircularProgressIndicator(
                              strokeWidth: 2,
                              color: Colors.white,
                            ),
                          )
                        : const Text("Continue"),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _saveAlbum(AlbumUpsertRequest request) async {
    if (_isSavingAlbum) return;

    setState(() {
      _isSavingAlbum = true;
    });

    try {
      final AlbumSaveResponse result =
          await _albumProvider.saveDeezerAlbum(request);

      if (!mounted) return;

      setState(() {
        _isSavingAlbum = false;
      });

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            'Album saved. New tracks: ${result.savedTracksCount}, existing tracks: ${result.existingTracksCount}.',
          ),
        ),
      );
    } catch (e) {
      if (!mounted) return;

      setState(() {
        _isSavingAlbum = false;
      });

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text("Error while saving album: $e"),
        ),
      );
    }
  }

  String _formatDuration(int? seconds) {
    if (seconds == null || seconds <= 0) return '--:--';

    final minutes = seconds ~/ 60;
    final remainingSeconds = seconds % 60;

    return '$minutes:${remainingSeconds.toString().padLeft(2, '0')}';
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(24),
      decoration: BoxDecoration(
        color: Colors.white,
        border: Border.all(color: MusicScreen.borderColor),
        borderRadius: BorderRadius.circular(14),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const _SectionHeader(
            title: "Add music",
            subtitle:
                "Add individual songs directly or browse albums and confirm their tracks through a preview dialog.",
          ),
          const SizedBox(height: 22),
          _ModeSwitcher(
            selectedMode: _mode,
            onChanged: (value) async {
              setState(() {
                _mode = value;
              });

              if (value == AddMusicMode.album &&
                  _deezerHelper.albumResults.isEmpty &&
                  !_deezerHelper.isLoadingAlbums) {
                await _deezerHelper.loadInitialAlbums();
              }
            },
          ),
          const SizedBox(height: 24),
          Expanded(
            child: _mode == AddMusicMode.song
                ? _buildSongLayout()
                : _buildAlbumLayout(),
          ),
        ],
      ),
    );
  }

  Widget _buildSongLayout() {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Expanded(
          flex: 6,
          child: _Panel(
            title: "Add songs",
            subtitle:
                "Search Deezer songs and prepare a list of songs you want to save.",
            child: Column(
              children: [
                Row(
                  children: [
                    Expanded(
                      child: _SearchBox(
                        controller: _songSearchController,
                        hintText: "Search song, artist or Deezer track id...",
                        onSubmitted: (_) => _searchSongs(),
                      ),
                    ),
                    const SizedBox(width: 12),
                    SizedBox(
                      height: 46,
                      child: ElevatedButton.icon(
                        onPressed:
                            _deezerHelper.isLoadingSongs ? null : _searchSongs,
                        icon: const Icon(Icons.search_rounded, size: 18),
                        label: const Text("Search"),
                        style: ElevatedButton.styleFrom(
                          backgroundColor: MusicScreen.primaryColor,
                          foregroundColor: Colors.white,
                          elevation: 0,
                          padding: const EdgeInsets.symmetric(horizontal: 18),
                        ),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 18),
                Expanded(
                  child: _buildSongResults(),
                ),
                if (_deezerHelper.songNextUrl != null) ...[
                  const SizedBox(height: 14),
                  Align(
                    alignment: Alignment.centerLeft,
                    child: OutlinedButton.icon(
                      onPressed: _deezerHelper.isLoadingSongs
                          ? null
                          : _deezerHelper.loadMoreSongs,
                      icon: const Icon(Icons.expand_more_rounded),
                      label: const Text("Load more"),
                      style: OutlinedButton.styleFrom(
                        side: const BorderSide(
                          color: MusicScreen.borderColor,
                        ),
                      ),
                    ),
                  ),
                ],
              ],
            ),
          ),
        ),
        const SizedBox(width: 18),
        Expanded(
          flex: 4,
          child: _Panel(
            title: "Selected songs",
            subtitle:
                "Songs that will be saved after confirmation. Review the final list here.",
            child: Column(
              children: [
                Expanded(
                  child: _selectedSongs.isEmpty
                      ? const _EmptyState(
                          title: "No songs selected",
                          subtitle:
                              "Select one or more songs from the left side.",
                        )
                      : ListView.separated(
                          itemCount: _selectedSongs.length,
                          separatorBuilder: (_, __) =>
                              const SizedBox(height: 10),
                          itemBuilder: (context, index) {
                            final song = _selectedSongs[index];
                            return _SelectedSongTile(
                              title: song.title,
                              artist: song.artist,
                              duration: song.duration,
                              coverUrl: song.coverUrl,
                              onRemove: () => _removeSongFromSelection(song.id),
                            );
                          },
                        ),
                ),
                const SizedBox(height: 18),
                Row(
                  children: [
                    Expanded(
                      child: OutlinedButton(
                        onPressed:
                            _selectedSongs.isEmpty ? null : _clearSelectedSongs,
                        style: OutlinedButton.styleFrom(
                          minimumSize: const Size.fromHeight(46),
                          side: const BorderSide(
                            color: MusicScreen.borderColor,
                          ),
                        ),
                        child: const Text("Clear list"),
                      ),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: ElevatedButton(
                        onPressed: (_selectedSongs.isEmpty || _isSavingSongs)
                            ? null
                            : _saveSongs,
                        style: ElevatedButton.styleFrom(
                          backgroundColor: MusicScreen.primaryColor,
                          foregroundColor: Colors.white,
                          minimumSize: const Size.fromHeight(46),
                          elevation: 0,
                        ),
                        child: _isSavingSongs
                            ? const SizedBox(
                                width: 18,
                                height: 18,
                                child: CircularProgressIndicator(
                                  strokeWidth: 2,
                                  color: Colors.white,
                                ),
                              )
                            : const Text("Save songs"),
                      ),
                    ),
                  ],
                ),
              ],
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildSongResults() {
    if (_deezerHelper.isLoadingSongs && _deezerHelper.songResults.isEmpty) {
      return const Center(
        child: CircularProgressIndicator(),
      );
    }

    if (_deezerHelper.songError != null &&
        _deezerHelper.songResults.isEmpty) {
      return _EmptyState(
        title: "Failed to load songs",
        subtitle: _deezerHelper.songError!,
      );
    }

    if (_deezerHelper.songResults.isEmpty) {
      return const _EmptyState(
        title: "No songs found",
        subtitle: "Try another search term.",
      );
    }

    return ListView.separated(
      itemCount: _deezerHelper.songResults.length,
      separatorBuilder: (_, __) => const SizedBox(height: 12),
      itemBuilder: (context, index) {
        final track = _deezerHelper.songResults[index];
        return _SongSearchCard(
          title: track.title,
          artist: track.artist?.name ?? 'Unknown artist',
          album: track.album?.title ?? 'Unknown album',
          duration: _formatDuration(track.duration),
          coverUrl: track.album?.coverMedium ?? track.album?.coverBig,
          onAdd: () => _addSongToSelection(track),
        );
      },
    );
  }

  Widget _buildAlbumLayout() {
    return _Panel(
      title: "Add albums",
      subtitle:
          "Browse suggested albums or search Deezer albums. Clicking Add should open a preview dialog with album tracks.",
      child: Column(
        children: [
          Row(
            children: [
              Expanded(
                child: _SearchBox(
                  controller: _albumSearchController,
                  hintText: "Search album, artist or Deezer album id...",
                  onSubmitted: (_) => _searchAlbums(),
                ),
              ),
              const SizedBox(width: 12),
              SizedBox(
                height: 46,
                child: ElevatedButton.icon(
                  onPressed:
                      _deezerHelper.isLoadingAlbums ? null : _searchAlbums,
                  icon: const Icon(Icons.search_rounded, size: 18),
                  label: const Text("Search"),
                  style: ElevatedButton.styleFrom(
                    backgroundColor: MusicScreen.primaryColor,
                    foregroundColor: Colors.white,
                    elevation: 0,
                    padding: const EdgeInsets.symmetric(horizontal: 18),
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 18),
          Expanded(
            child: _buildAlbumResults(),
          ),
          if (_deezerHelper.albumNextUrl != null) ...[
            const SizedBox(height: 14),
            Align(
              alignment: Alignment.centerLeft,
              child: OutlinedButton.icon(
                onPressed: _deezerHelper.isLoadingAlbums
                    ? null
                    : _deezerHelper.loadMoreAlbums,
                icon: const Icon(Icons.expand_more_rounded),
                label: const Text("Load more"),
                style: OutlinedButton.styleFrom(
                  side: const BorderSide(
                    color: MusicScreen.borderColor,
                  ),
                ),
              ),
            ),
          ],
        ],
      ),
    );
  }

  Widget _buildAlbumResults() {
    if (_deezerHelper.isLoadingAlbums && _deezerHelper.albumResults.isEmpty) {
      return const Center(
        child: CircularProgressIndicator(),
      );
    }

    if (_deezerHelper.albumError != null &&
        _deezerHelper.albumResults.isEmpty) {
      return _EmptyState(
        title: "Failed to load albums",
        subtitle: _deezerHelper.albumError!,
      );
    }

    if (_deezerHelper.albumResults.isEmpty) {
      return const _EmptyState(
        title: "No albums found",
        subtitle: "Search albums to begin.",
      );
    }

    return ListView.separated(
      itemCount: _deezerHelper.albumResults.length,
      separatorBuilder: (_, __) => const SizedBox(height: 12),
      itemBuilder: (context, index) {
        final album = _deezerHelper.albumResults[index];
        return _AlbumSearchCard(
          title: album.title,
          artist: album.artist?.name ?? 'Unknown artist',
          trackCountLabel: album.releaseDate ?? 'Album',
          year: album.releaseDate?.split('-').first ?? '-',
          coverUrl: album.coverMedium ?? album.coverBig,
          isLoading: _deezerHelper.isLoadingAlbumDetails || _isSavingAlbum,
          onAdd: () => _openAlbumPreview(album),
        );
      },
    );
  }
}

class _ModeSwitcher extends StatelessWidget {
  final AddMusicMode selectedMode;
  final ValueChanged<AddMusicMode> onChanged;

  const _ModeSwitcher({
    required this.selectedMode,
    required this.onChanged,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        _ModeButton(
          title: "Add Song",
          icon: Icons.music_note_rounded,
          active: selectedMode == AddMusicMode.song,
          onTap: () => onChanged(AddMusicMode.song),
        ),
        const SizedBox(width: 12),
        _ModeButton(
          title: "Add Album",
          icon: Icons.album_rounded,
          active: selectedMode == AddMusicMode.album,
          onTap: () => onChanged(AddMusicMode.album),
        ),
      ],
    );
  }
}

class _ModeButton extends StatelessWidget {
  final String title;
  final IconData icon;
  final bool active;
  final VoidCallback onTap;

  const _ModeButton({
    required this.title,
    required this.icon,
    required this.active,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(12),
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 180),
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
        decoration: BoxDecoration(
          color: active ? MusicScreen.primaryLight : Colors.white,
          borderRadius: BorderRadius.circular(12),
          border: Border.all(
            color: active
                ? MusicScreen.primaryColor
                : MusicScreen.borderColor,
          ),
        ),
        child: Row(
          children: [
            Icon(
              icon,
              size: 18,
              color: active
                  ? MusicScreen.primaryColor
                  : MusicScreen.subTextColor,
            ),
            const SizedBox(width: 8),
            Text(
              title,
              style: TextStyle(
                fontSize: 13,
                fontWeight: FontWeight.w700,
                color: active
                    ? MusicScreen.primaryColor
                    : MusicScreen.textColor,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _Panel extends StatelessWidget {
  final String title;
  final String subtitle;
  final Widget child;

  const _Panel({
    required this.title,
    required this.subtitle,
    required this.child,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      height: double.infinity,
      padding: const EdgeInsets.all(18),
      decoration: BoxDecoration(
        color: Colors.white,
        border: Border.all(color: MusicScreen.borderColor),
        borderRadius: BorderRadius.circular(14),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            title,
            style: const TextStyle(
              fontSize: 18,
              fontWeight: FontWeight.w800,
              color: MusicScreen.textColor,
            ),
          ),
          const SizedBox(height: 6),
          Text(
            subtitle,
            style: const TextStyle(
              fontSize: 13,
              height: 1.5,
              color: MusicScreen.subTextColor,
            ),
          ),
          const SizedBox(height: 18),
          Expanded(child: child),
        ],
      ),
    );
  }
}

class _SectionHeader extends StatelessWidget {
  final String title;
  final String subtitle;

  const _SectionHeader({
    required this.title,
    required this.subtitle,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          title,
          style: const TextStyle(
            fontSize: 28,
            fontWeight: FontWeight.w800,
            color: MusicScreen.textColor,
          ),
        ),
        const SizedBox(height: 8),
        Text(
          subtitle,
          style: const TextStyle(
            fontSize: 14,
            height: 1.5,
            color: MusicScreen.subTextColor,
          ),
        ),
      ],
    );
  }
}

class _SearchBox extends StatelessWidget {
  final TextEditingController controller;
  final String hintText;
  final ValueChanged<String>? onSubmitted;

  const _SearchBox({
    required this.controller,
    required this.hintText,
    this.onSubmitted,
  });

  @override
  Widget build(BuildContext context) {
    return TextField(
      controller: controller,
      onSubmitted: onSubmitted,
      decoration: InputDecoration(
        hintText: hintText,
        prefixIcon: const Icon(Icons.search_rounded),
        filled: true,
        fillColor: const Color(0xFFF8F8FA),
        contentPadding: const EdgeInsets.symmetric(
          horizontal: 14,
          vertical: 14,
        ),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: const BorderSide(color: MusicScreen.borderColor),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: const BorderSide(color: MusicScreen.borderColor),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: const BorderSide(
            color: MusicScreen.primaryColor,
            width: 1.2,
          ),
        ),
      ),
    );
  }
}

class _SongSearchCard extends StatelessWidget {
  final String title;
  final String artist;
  final String album;
  final String duration;
  final String? coverUrl;
  final VoidCallback onAdd;

  const _SongSearchCard({
    required this.title,
    required this.artist,
    required this.album,
    required this.duration,
    required this.coverUrl,
    required this.onAdd,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Colors.white,
        border: Border.all(color: MusicScreen.borderColor),
        borderRadius: BorderRadius.circular(12),
      ),
      child: Row(
        children: [
          _CoverImage(
            icon: Icons.music_note_rounded,
            imageUrl: coverUrl,
            size: 52,
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  title,
                  style: const TextStyle(
                    fontSize: 14,
                    fontWeight: FontWeight.w700,
                    color: MusicScreen.textColor,
                  ),
                ),
                const SizedBox(height: 4),
                Text(
                  "$artist • $album",
                  style: const TextStyle(
                    fontSize: 12,
                    color: MusicScreen.subTextColor,
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(width: 10),
          Text(
            duration,
            style: const TextStyle(
              fontSize: 12,
              fontWeight: FontWeight.w600,
              color: MusicScreen.subTextColor,
            ),
          ),
          const SizedBox(width: 10),
          ElevatedButton.icon(
            onPressed: onAdd,
            icon: const Icon(Icons.add_rounded, size: 18),
            label: const Text("Add"),
            style: ElevatedButton.styleFrom(
              elevation: 0,
              backgroundColor: MusicScreen.primaryColor,
              foregroundColor: Colors.white,
            ),
          ),
        ],
      ),
    );
  }
}

class _AlbumSearchCard extends StatelessWidget {
  final String title;
  final String artist;
  final String trackCountLabel;
  final String year;
  final String? coverUrl;
  final bool isLoading;
  final VoidCallback onAdd;

  const _AlbumSearchCard({
    required this.title,
    required this.artist,
    required this.trackCountLabel,
    required this.year,
    required this.coverUrl,
    required this.isLoading,
    required this.onAdd,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Colors.white,
        border: Border.all(color: MusicScreen.borderColor),
        borderRadius: BorderRadius.circular(12),
      ),
      child: Row(
        children: [
          _CoverImage(
            icon: Icons.album_rounded,
            imageUrl: coverUrl,
            size: 62,
          ),
          const SizedBox(width: 14),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  title,
                  style: const TextStyle(
                    fontSize: 15,
                    fontWeight: FontWeight.w800,
                    color: MusicScreen.textColor,
                  ),
                ),
                const SizedBox(height: 4),
                Text(
                  artist,
                  style: const TextStyle(
                    fontSize: 12,
                    color: MusicScreen.subTextColor,
                  ),
                ),
                const SizedBox(height: 8),
                Wrap(
                  spacing: 8,
                  runSpacing: 8,
                  children: [
                    _InfoChip(label: trackCountLabel),
                    _InfoChip(label: year),
                  ],
                ),
              ],
            ),
          ),
          const SizedBox(width: 12),
          ElevatedButton.icon(
            onPressed: isLoading ? null : onAdd,
            icon: isLoading
                ? const SizedBox(
                    width: 16,
                    height: 16,
                    child: CircularProgressIndicator(strokeWidth: 2),
                  )
                : const Icon(Icons.add_rounded, size: 18),
            label: Text(isLoading ? "Loading..." : "Add"),
            style: ElevatedButton.styleFrom(
              backgroundColor: MusicScreen.primaryColor,
              foregroundColor: Colors.white,
              elevation: 0,
              minimumSize: const Size(110, 42),
            ),
          ),
        ],
      ),
    );
  }
}

class _InfoChip extends StatelessWidget {
  final String label;

  const _InfoChip({
    required this.label,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
      decoration: BoxDecoration(
        color: const Color(0xFFF7F7FA),
        borderRadius: BorderRadius.circular(999),
        border: Border.all(color: MusicScreen.borderColor),
      ),
      child: Text(
        label,
        style: const TextStyle(
          fontSize: 11,
          fontWeight: FontWeight.w700,
          color: MusicScreen.textColor,
        ),
      ),
    );
  }
}

class _SelectedSongTile extends StatelessWidget {
  final String title;
  final String artist;
  final String duration;
  final String? coverUrl;
  final VoidCallback onRemove;

  const _SelectedSongTile({
    required this.title,
    required this.artist,
    required this.duration,
    required this.coverUrl,
    required this.onRemove,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(13),
      decoration: BoxDecoration(
        color: const Color(0xFFFCFCFD),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: MusicScreen.borderColor),
      ),
      child: Row(
        children: [
          _CoverImage(
            icon: Icons.library_music_rounded,
            imageUrl: coverUrl,
            size: 42,
          ),
          const SizedBox(width: 10),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  title,
                  style: const TextStyle(
                    fontSize: 13,
                    fontWeight: FontWeight.w700,
                    color: MusicScreen.textColor,
                  ),
                ),
                const SizedBox(height: 2),
                Text(
                  artist,
                  style: const TextStyle(
                    fontSize: 12,
                    color: MusicScreen.subTextColor,
                  ),
                ),
              ],
            ),
          ),
          Text(
            duration,
            style: const TextStyle(
              fontSize: 12,
              color: MusicScreen.subTextColor,
            ),
          ),
          IconButton(
            onPressed: onRemove,
            icon: const Icon(
              Icons.close_rounded,
              size: 18,
              color: MusicScreen.subTextColor,
            ),
          ),
        ],
      ),
    );
  }
}

class _ExistingSongTile extends StatelessWidget {
  final dynamic song;

  const _ExistingSongTile({
    required this.song,
  });

  @override
  Widget build(BuildContext context) {
    final String title = song.title ?? '';
    final String artistName = song.artistName ?? '';
    final String? albumTitle = song.albumTitle;
    final String? coverUrl = song.coverUrl;

    return Container(
      padding: const EdgeInsets.all(13),
      decoration: BoxDecoration(
        color: const Color(0xFFFCFCFD),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: MusicScreen.borderColor),
      ),
      child: Row(
        children: [
          _CoverImage(
            icon: Icons.music_note_rounded,
            imageUrl: coverUrl,
            size: 44,
          ),
          const SizedBox(width: 10),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  title,
                  style: const TextStyle(
                    fontSize: 13,
                    fontWeight: FontWeight.w700,
                    color: MusicScreen.textColor,
                  ),
                ),
                const SizedBox(height: 3),
                Text(
                  albumTitle != null && albumTitle.isNotEmpty
                      ? "$artistName • $albumTitle"
                      : artistName,
                  style: const TextStyle(
                    fontSize: 12,
                    color: MusicScreen.subTextColor,
                  ),
                ),
              ],
            ),
          ),
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
            decoration: BoxDecoration(
              color: const Color(0xFFFFF0F0),
              borderRadius: BorderRadius.circular(999),
            ),
            child: const Text(
              "Already exists",
              style: TextStyle(
                fontSize: 11,
                fontWeight: FontWeight.w700,
                color: Color(0xFFC62828),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _CoverImage extends StatelessWidget {
  final IconData icon;
  final String? imageUrl;
  final double size;

  const _CoverImage({
    required this.icon,
    required this.imageUrl,
    required this.size,
  });

  @override
  Widget build(BuildContext context) {
    final hasImage = imageUrl != null && imageUrl!.trim().isNotEmpty;

    return Container(
      width: size,
      height: size,
      decoration: BoxDecoration(
        color: MusicScreen.primaryLight,
        borderRadius: BorderRadius.circular(12),
        image: hasImage
            ? DecorationImage(
                image: NetworkImage(imageUrl!),
                fit: BoxFit.cover,
              )
            : null,
      ),
      child: hasImage
          ? null
          : Icon(
              icon,
              color: MusicScreen.primaryColor,
              size: size * 0.45,
            ),
    );
  }
}

class _EmptyState extends StatelessWidget {
  final String title;
  final String subtitle;

  const _EmptyState({
    required this.title,
    required this.subtitle,
  });

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            const Icon(
              Icons.queue_music_rounded,
              size: 46,
              color: MusicScreen.subTextColor,
            ),
            const SizedBox(height: 14),
            Text(
              title,
              style: const TextStyle(
                fontSize: 16,
                fontWeight: FontWeight.w800,
                color: MusicScreen.textColor,
              ),
            ),
            const SizedBox(height: 8),
            Text(
              subtitle,
              textAlign: TextAlign.center,
              style: const TextStyle(
                fontSize: 13,
                height: 1.5,
                color: MusicScreen.subTextColor,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _SongPreviewModel {
  final int id;
  final String externalTrackId;
  final String title;
  final String artist;
  final String duration;
  final int durationSeconds;
  final String? coverUrl;
  final String? previewUrl;
  final String? albumTitle;

  _SongPreviewModel({
    required this.id,
    required this.externalTrackId,
    required this.title,
    required this.artist,
    required this.duration,
    required this.durationSeconds,
    this.coverUrl,
    this.previewUrl,
    this.albumTitle,
  });
}