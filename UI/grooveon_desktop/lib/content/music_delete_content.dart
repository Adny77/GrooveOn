import 'dart:async';
import 'package:flutter/material.dart';
import 'package:grooveon_desktop/dialogs/confirmation_dialogs.dart';
import 'package:grooveon_desktop/helper/snackBar_helper.dart';
import 'package:grooveon_desktop/helper/universal_paging_helper.dart';
import 'package:grooveon_desktop/models/response/album_response.dart';
import 'package:grooveon_desktop/models/response/song_response.dart';
import 'package:grooveon_desktop/providers/album_provider.dart';
import 'package:grooveon_desktop/providers/song_provider.dart';
import 'package:grooveon_desktop/screens/music_screen.dart';

enum DeleteMusicMode { song, album }

class MusicDeleteContent extends StatefulWidget {
  const MusicDeleteContent({super.key});

  @override
  State<MusicDeleteContent> createState() => _MusicDeleteContentState();
}

class _MusicDeleteContentState extends State<MusicDeleteContent> {
  DeleteMusicMode _mode = DeleteMusicMode.song;

  final TextEditingController _songSearchController = TextEditingController();
  final TextEditingController _albumSearchController = TextEditingController();

  late final SongProvider _songProvider;
  late final AlbumProvider _albumProvider;

  late final UniversalPagingProvider<SongResponse> _songPaging;
  late final UniversalPagingProvider<AlbumResponse> _albumPaging;

  final List<SongResponse> _selectedSongs = [];

  bool _isDeletingSongs = false;
  int? _deletingAlbumId;

  @override
  void initState() {
    super.initState();

    _songProvider = SongProvider();
    _albumProvider = AlbumProvider();

    _songPaging = UniversalPagingProvider<SongResponse>(
      pageSize: 5,
      fetcher: ({
        required int page,
        required int pageSize,
        String? filter,
        bool includeTotalCount = true,
      }) {
        return _songProvider.getPaged(
          page: page,
          pageSize: pageSize,
          filter: filter,
          includeTotalCount: includeTotalCount,
        );
      },
    );

    _albumPaging = UniversalPagingProvider<AlbumResponse>(
      pageSize: 5,
      fetcher: ({
        required int page,
        required int pageSize,
        String? filter,
        bool includeTotalCount = true,
      }) {
        return _albumProvider.getPaged(
          page: page,
          pageSize: pageSize,
          filter: filter,
          includeTotalCount: includeTotalCount,
        );
      },
    );

    WidgetsBinding.instance.addPostFrameCallback((_) async {
      await _loadInitialData();
    });
  }

  Future<void> _loadInitialData() async {
    await Future.wait([
      _songPaging.loadPage(),
      _albumPaging.loadPage(),
    ]);
  }

  @override
  void dispose() {
    _songSearchController.dispose();
    _albumSearchController.dispose();
    _songPaging.dispose();
    _albumPaging.dispose();
    super.dispose();
  }

  Future<void> _searchSongs() async {
    await _songPaging.search(_songSearchController.text.trim());
  }

  Future<void> _searchAlbums() async {
    await _albumPaging.search(_albumSearchController.text.trim());
  }

  void _addSongToSelection(SongResponse song) {
    final exists = _selectedSongs.any((x) => x.id == song.id);
    if (exists) {
      SnackbarHelper.showInfo(context, "Song is already added to the delete list.");
      return;
    }

    setState(() {
      _selectedSongs.add(song);
    });
  }

  void _removeSongFromSelection(int songId) {
    setState(() {
      _selectedSongs.removeWhere((x) => x.id == songId);
    });
  }

  void _clearSelectedSongs() {
    setState(() {
      _selectedSongs.clear();
    });
  }

  Future<void> _deleteSelectedSongs() async {
    if (_selectedSongs.isEmpty || _isDeletingSongs) return;

    final confirmed = await ConfirmDialogs.yesNoConfirmation(
      context,
      title: "Confirmation",
      question: "Are you sure you want to delete all selected songs?",
    );

    if (confirmed != true) return;

    setState(() {
      _isDeletingSongs = true;
    });

    try {
      final songsToDelete = List<SongResponse>.from(_selectedSongs);

      for (final song in songsToDelete) {
        await _songProvider.delete(song.id!);
      }

      if (!mounted) return;

      setState(() {
        _selectedSongs.clear();
      });

      await Future.wait([_songPaging.refresh(), _albumPaging.refresh()]);

      if (!mounted) return;

      SnackbarHelper.showSuccess(context, "${songsToDelete.length} song(s) deleted successfully.");
    } catch (e) {
      if (!mounted) return;

      SnackbarHelper.showError(context, e.toString());
    } finally {
      if (mounted) {
        setState(() {
          _isDeletingSongs = false;
        });
      }
    }
  }

  Future<void> _deleteAlbum(AlbumResponse album) async {
    if (_deletingAlbumId != null) return;

    final confirmed = await ConfirmDialogs.yesNoConfirmation(
      context,
      title: "Confirmation",
      question: "Are you sure you want to delete this album?",
    );

    if (confirmed != true) return;

    setState(() {
      _deletingAlbumId = album.id;
    });

    try {
      await _albumProvider.delete(album.id!);

      if (!mounted) return;

      await Future.wait([_albumPaging.refresh(), _songPaging.refresh()]);

      if (!mounted) return;

      _selectedSongs.removeWhere((x) => x.albumTitle == album.title);

      SnackbarHelper.showSuccess(context, "Album '${album.title}' deleted successfully.");
    } catch (e) {
      if (!mounted) return;

      SnackbarHelper.showError(context, e.toString());
    } finally {
      if (mounted) {
        setState(() {
          _deletingAlbumId = null;
        });
      }
    }
  }

  String _formatDuration(int? seconds) {
    if (seconds == null || seconds <= 0) return '--:--';

    final minutes = seconds ~/ 60;
    final remainingSeconds = seconds % 60;

    return '$minutes:${remainingSeconds.toString().padLeft(2, '0')}';
  }

  String _resolveAlbumArtistName(AlbumResponse album) {
    final dynamic a = album;
    return (a.artistName ?? a.artist?.name ?? 'Unknown artist').toString();
  }

  int _resolveAlbumTrackCount(AlbumResponse album) {
    final dynamic a = album;
    return (a.songCount as int?) ??
        (a.numberOfTracks as int?) ??
        0;
  }

  String? _resolveAlbumYear(AlbumResponse album) {
    final dynamic a = album;
    final releaseDate = a.releaseDate?.toString();

    if (releaseDate == null || releaseDate.isEmpty) return null;
    return releaseDate.length >= 4 ? releaseDate.substring(0, 4) : releaseDate;
  }

  String? _resolveAlbumCoverUrl(AlbumResponse album) {
    final dynamic a = album;
    return (a.coverUrl ??
            a.imageUrl ??
            a.coverMedium ??
            a.coverBig)
        ?.toString();
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
            title: "Delete music",
            subtitle:
                "Delete songs through a selected list or remove albums directly from the results.",
          ),
          const SizedBox(height: 22),
          _ModeSwitcher(
            selectedMode: _mode,
            onChanged: (value) {
              setState(() {
                _mode = value;
              });
            },
          ),
          const SizedBox(height: 24),
          Expanded(
            child: _mode == DeleteMusicMode.song
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
          child: AnimatedBuilder(
            animation: _songPaging,
            builder: (context, _) => _PanelWithAction(
              title: "Delete songs",
              subtitle:
                  "Search songs from your system and add them to the delete list.",
              action: _RefreshIconButton(
                isLoading: _songPaging.isLoading,
                onPressed: () async {
                  _songSearchController.clear();
                  _clearSelectedSongs();
                  await _songPaging.loadPage(pageNumber: 0, filter: " ");
                },
              ),
              child: Column(
                children: [
                  Row(
                    children: [
                      Expanded(
                        child: _SearchBox(
                          controller: _songSearchController,
                          hintText: "Search song or artist...",
                          onSubmitted: (_) => _searchSongs(),
                        ),
                      ),
                      const SizedBox(width: 12),
                      SizedBox(
                        height: 46,
                        child: ElevatedButton.icon(
                          onPressed: _songPaging.isLoading ? null : _searchSongs,
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
                  const SizedBox(height: 16),
                  _PagingControls(
                    page: _songPaging.page,
                    hasNextPage: _songPaging.hasNextPage,
                    hasPreviousPage: _songPaging.hasPreviousPage,
                    totalCount: _songPaging.totalCount,
                    pageSize: _songPaging.pageSize,
                    isLoading: _songPaging.isLoading,
                    onPrevious: _songPaging.previousPage,
                    onNext: _songPaging.nextPage,
                  ),
                ],
              ),
            ),
          ),
        ),
        const SizedBox(width: 18),
        Expanded(
          flex: 4,
          child: _PanelWithAction(
            title: "Selected songs",
            subtitle: "Songs that will be deleted after confirmation.",
            child: Column(
              children: [
                Expanded(
                  child: _selectedSongs.isEmpty
                      ? const _EmptyState(
                          title: "No songs selected",
                          subtitle: "Select songs from the left side.",
                        )
                      : ListView.separated(
                          itemCount: _selectedSongs.length,
                          separatorBuilder: (_, __) => const SizedBox(height: 10),
                          itemBuilder: (context, index) {
                            final song = _selectedSongs[index];
                            return _SelectedSongTile(
                              title: song.title ?? '',
                              artist: song.artistName ?? 'Unknown artist',
                              duration: _formatDuration(song.durationSeconds),
                              coverUrl: song.coverUrl,
                              onRemove: () => _removeSongFromSelection(song.id!),
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
                        onPressed:
                            (_selectedSongs.isEmpty || _isDeletingSongs)
                                ? null
                                : _deleteSelectedSongs,
                        style: ElevatedButton.styleFrom(
                          backgroundColor: const Color(0xFFE14D43),
                          foregroundColor: Colors.white,
                          minimumSize: const Size.fromHeight(46),
                          elevation: 0,
                        ),
                        child: _isDeletingSongs
                            ? const SizedBox(
                                width: 18,
                                height: 18,
                                child: CircularProgressIndicator(
                                  strokeWidth: 2,
                                  color: Colors.white,
                                ),
                              )
                            : const Text("Delete songs"),
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
    if (_songPaging.isLoading && _songPaging.items.isEmpty) {
      return const Center(child: CircularProgressIndicator());
    }

    if (_songPaging.items.isEmpty) {
      return const _EmptyState(
        title: "No songs found",
        subtitle: "Try another search term.",
      );
    }

    return ListView.separated(
      itemCount: _songPaging.items.length,
      separatorBuilder: (_, __) => const SizedBox(height: 12),
      itemBuilder: (context, index) {
        final song = _songPaging.items[index];
        final isSelected = _selectedSongs.any((x) => x.id == song.id);

        return _SongDeleteCard(
          title: song.title ?? '',
          artist: song.artistName ?? 'Unknown artist',
          album: song.albumTitle ?? 'Unknown album',
          duration: _formatDuration(song.durationSeconds),
          coverUrl: song.coverUrl,
          isSelected: isSelected,
          onSelect: () => _addSongToSelection(song),
        );
      },
    );
  }

  Widget _buildAlbumLayout() {
    return _PanelWithAction(
      title: "Delete albums",
      subtitle:
          "Browse albums from your system and delete them directly from the list.",
      action: _RefreshIconButton(
        isLoading: _albumPaging.isLoading,
        onPressed: () async {
          _albumSearchController.clear();
          await _albumPaging.loadPage(pageNumber: 0, filter: " ");
        },
      ),
      child: Column(
        children: [
          Row(
            children: [
              Expanded(
                child: _SearchBox(
                  controller: _albumSearchController,
                  hintText: "Search album or artist...",
                  onSubmitted: (_) => _searchAlbums(),
                ),
              ),
              const SizedBox(width: 12),
              SizedBox(
                height: 46,
                child: ElevatedButton.icon(
                  onPressed: _albumPaging.isLoading ? null : _searchAlbums,
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
            child: AnimatedBuilder(
              animation: _albumPaging,
              builder: (context, _) => _buildAlbumResults(),
            ),
          ),
          const SizedBox(height: 16),
          AnimatedBuilder(
            animation: _albumPaging,
            builder: (context, _) => _PagingControls(
              page: _albumPaging.page,
              hasNextPage: _albumPaging.hasNextPage,
              hasPreviousPage: _albumPaging.hasPreviousPage,
              totalCount: _albumPaging.totalCount,
              pageSize: _albumPaging.pageSize,
              isLoading: _albumPaging.isLoading,
              onPrevious: _albumPaging.previousPage,
              onNext: _albumPaging.nextPage,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildAlbumResults() {
    if (_albumPaging.isLoading && _albumPaging.items.isEmpty) {
      return const Center(child: CircularProgressIndicator());
    }

    if (_albumPaging.items.isEmpty) {
      return const _EmptyState(
        title: "No albums found",
        subtitle: "Try another search term.",
      );
    }

    return ListView.separated(
      itemCount: _albumPaging.items.length,
      separatorBuilder: (_, __) => const SizedBox(height: 12),
      itemBuilder: (context, index) {
        final album = _albumPaging.items[index];
        final trackCount = _resolveAlbumTrackCount(album);

        return _AlbumDeleteCard(
          title: album.title ?? '',
          artist: _resolveAlbumArtistName(album),
          trackCountLabel: trackCount > 0 ? "$trackCount tracks" : "Album",
          year: _resolveAlbumYear(album) ?? '-',
          coverUrl: _resolveAlbumCoverUrl(album),
          isLoading: _deletingAlbumId == album.id,
          onDelete: () => _deleteAlbum(album),
        );
      },
    );
  }
}

class _ModeSwitcher extends StatelessWidget {
  final DeleteMusicMode selectedMode;
  final ValueChanged<DeleteMusicMode> onChanged;

  const _ModeSwitcher({required this.selectedMode, required this.onChanged});

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        _ModeButton(
          title: "Delete Song",
          icon: Icons.music_note_rounded,
          active: selectedMode == DeleteMusicMode.song,
          onTap: () => onChanged(DeleteMusicMode.song),
        ),
        const SizedBox(width: 12),
        _ModeButton(
          title: "Delete Album",
          icon: Icons.album_rounded,
          active: selectedMode == DeleteMusicMode.album,
          onTap: () => onChanged(DeleteMusicMode.album),
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
            color: active ? MusicScreen.primaryColor : MusicScreen.borderColor,
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

class _PanelWithAction extends StatelessWidget {
  final String title;
  final String subtitle;
  final Widget child;
  final Widget? action;

  const _PanelWithAction({
    required this.title,
    required this.subtitle,
    required this.child,
    this.action,
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
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Expanded(
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
                  ],
                ),
              ),
              if (action != null) ...[const SizedBox(width: 12), action!],
            ],
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

  const _SectionHeader({required this.title, required this.subtitle});

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

class _SongDeleteCard extends StatelessWidget {
  final String title;
  final String artist;
  final String album;
  final String duration;
  final String? coverUrl;
  final bool isSelected;
  final VoidCallback onSelect;

  const _SongDeleteCard({
    required this.title,
    required this.artist,
    required this.album,
    required this.duration,
    required this.coverUrl,
    required this.isSelected,
    required this.onSelect,
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
            onPressed: onSelect,
            icon: Icon(
              isSelected ? Icons.check_rounded : Icons.playlist_add_rounded,
              size: 18,
            ),
            label: Text(isSelected ? "Selected" : "Select"),
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

class _AlbumDeleteCard extends StatelessWidget {
  final String title;
  final String artist;
  final String trackCountLabel;
  final String year;
  final String? coverUrl;
  final bool isLoading;
  final VoidCallback onDelete;

  const _AlbumDeleteCard({
    required this.title,
    required this.artist,
    required this.trackCountLabel,
    required this.year,
    required this.coverUrl,
    required this.isLoading,
    required this.onDelete,
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
          _CoverImage(icon: Icons.album_rounded, imageUrl: coverUrl, size: 62),
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
            onPressed: isLoading ? null : onDelete,
            icon: isLoading
                ? const SizedBox(
                    width: 16,
                    height: 16,
                    child: CircularProgressIndicator(
                      strokeWidth: 2,
                      color: Colors.white,
                    ),
                  )
                : const Icon(Icons.delete_outline_rounded, size: 18),
            label: Text(isLoading ? "Deleting..." : "Delete"),
            style: ElevatedButton.styleFrom(
              backgroundColor: const Color(0xFFE14D43),
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

  const _InfoChip({required this.label});

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

class _PagingControls extends StatelessWidget {
  final int page;
  final int pageSize;
  final int totalCount;
  final bool hasPreviousPage;
  final bool hasNextPage;
  final bool isLoading;
  final Future<void> Function() onPrevious;
  final Future<void> Function() onNext;

  const _PagingControls({
    required this.page,
    required this.pageSize,
    required this.totalCount,
    required this.hasPreviousPage,
    required this.hasNextPage,
    required this.isLoading,
    required this.onPrevious,
    required this.onNext,
  });

  @override
  Widget build(BuildContext context) {
    final from = totalCount == 0 ? 0 : (page * pageSize) + 1;
    final to = ((page + 1) * pageSize) > totalCount
        ? totalCount
        : ((page + 1) * pageSize);

    return Row(
      children: [
        Text(
          totalCount == 0 ? "No records" : "Showing $from-$to of $totalCount",
          style: const TextStyle(fontSize: 12, color: MusicScreen.subTextColor),
        ),
        const Spacer(),
        OutlinedButton.icon(
          onPressed: (!hasPreviousPage || isLoading)
              ? null
              : () => onPrevious(),
          icon: const Icon(Icons.chevron_left_rounded),
          label: const Text("Previous"),
          style: OutlinedButton.styleFrom(
            side: const BorderSide(color: MusicScreen.borderColor),
          ),
        ),
        const SizedBox(width: 8),
        OutlinedButton.icon(
          onPressed: (!hasNextPage || isLoading) ? null : () => onNext(),
          icon: const Icon(Icons.chevron_right_rounded),
          label: const Text("Next"),
          style: OutlinedButton.styleFrom(
            side: const BorderSide(color: MusicScreen.borderColor),
          ),
        ),
      ],
    );
  }
}

class _EmptyState extends StatelessWidget {
  final String title;
  final String subtitle;

  const _EmptyState({required this.title, required this.subtitle});

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

class _RefreshIconButton extends StatelessWidget {
  final VoidCallback? onPressed;
  final bool isLoading;

  const _RefreshIconButton({required this.onPressed, this.isLoading = false});

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: 46,
      height: 46,
      child: OutlinedButton(
        onPressed: isLoading ? null : onPressed,
        style: OutlinedButton.styleFrom(
          padding: EdgeInsets.zero,
          side: const BorderSide(color: MusicScreen.borderColor),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(12),
          ),
          backgroundColor: Colors.white,
        ),
        child: isLoading
            ? const SizedBox(
                width: 18,
                height: 18,
                child: CircularProgressIndicator(strokeWidth: 2),
              )
            : const Icon(
                Icons.refresh_rounded,
                size: 20,
                color: MusicScreen.textColor,
              ),
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
          : Icon(icon, color: MusicScreen.primaryColor, size: size * 0.45),
    );
  }
}