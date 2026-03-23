import 'package:flutter/material.dart';
import 'package:grooveon_desktop/dialogs/confirmation_dialogs.dart';
import 'package:grooveon_desktop/screens/music_screen.dart';

enum DeleteMusicMode {
  song,
  album,
}

class SongDeleteItem {
  final int id;
  final String title;
  final String artistName;
  final String? albumTitle;
  final int durationSeconds;
  final String? coverUrl;

  SongDeleteItem({
    required this.id,
    required this.title,
    required this.artistName,
    this.albumTitle,
    required this.durationSeconds,
    this.coverUrl,
  });
}

class AlbumDeleteItem {
  final int id;
  final String title;
  final String artistName;
  final int trackCount;
  final String? coverUrl;
  final String? year;

  AlbumDeleteItem({
    required this.id,
    required this.title,
    required this.artistName,
    required this.trackCount,
    this.coverUrl,
    this.year,
  });
}

class MusicDeleteContent extends StatefulWidget {
  const MusicDeleteContent({super.key});

  @override
  State<MusicDeleteContent> createState() => _MusicDeleteContentState();
}

class _MusicDeleteContentState extends State<MusicDeleteContent> {
  DeleteMusicMode _mode = DeleteMusicMode.song;

  final TextEditingController _songSearchController = TextEditingController();
  final TextEditingController _albumSearchController = TextEditingController();

  final List<SongDeleteItem> _allSongs = [
    SongDeleteItem(
      id: 1,
      title: "Moja Lelo",
      artistName: "Trile",
      albumTitle: "Moja Lelo",
      durationSeconds: 174,
      coverUrl:
          "https://e-cdns-images.dzcdn.net/images/cover/55d9222c9cc298245f2f7e1a2b3a6902/250x250-000000-80-0-0.jpg",
    ),
    SongDeleteItem(
      id: 2,
      title: "Ride It",
      artistName: "Jay Sean",
      albumTitle: "Ride It",
      durationSeconds: 193,
      coverUrl:
          "https://e-cdns-images.dzcdn.net/images/cover/d0a7f4a5f55d22e23d3f8f87b0f201df/250x250-000000-80-0-0.jpg",
    ),
    SongDeleteItem(
      id: 3,
      title: "Twinkle Twinkle Little Star",
      artistName: "Kid Songs",
      albumTitle: "Kids Collection",
      durationSeconds: 142,
      coverUrl: null,
    ),
    SongDeleteItem(
      id: 4,
      title: "Blinding Lights",
      artistName: "The Weeknd",
      albumTitle: "After Hours",
      durationSeconds: 200,
      coverUrl: null,
    ),
  ];

  final List<AlbumDeleteItem> _allAlbums = [
    AlbumDeleteItem(
      id: 1,
      title: "After Hours",
      artistName: "The Weeknd",
      trackCount: 14,
      year: "2020",
      coverUrl: null,
    ),
    AlbumDeleteItem(
      id: 2,
      title: "Starboy",
      artistName: "The Weeknd",
      trackCount: 18,
      year: "2016",
      coverUrl: null,
    ),
    AlbumDeleteItem(
      id: 3,
      title: "Evolve",
      artistName: "Imagine Dragons",
      trackCount: 12,
      year: "2017",
      coverUrl: null,
    ),
  ];

  late List<SongDeleteItem> _songs;
  late List<AlbumDeleteItem> _albums;

  SongDeleteItem? _selectedSong;
  AlbumDeleteItem? _selectedAlbum;

  bool _isDeletingSong = false;
  bool _isDeletingAlbum = false;

  @override
  void initState() {
    super.initState();
    _songs = List.from(_allSongs);
    _albums = List.from(_allAlbums);

    _songSearchController.addListener(_filterSongs);
    _albumSearchController.addListener(_filterAlbums);
  }

  @override
  void dispose() {
    _songSearchController.dispose();
    _albumSearchController.dispose();
    super.dispose();
  }

  void _filterSongs() {
    final query = _songSearchController.text.trim().toLowerCase();

    setState(() {
      _songs = _allSongs.where((song) {
        if (query.isEmpty) return true;
        return song.title.toLowerCase().contains(query) ||
            song.artistName.toLowerCase().contains(query) ||
            (song.albumTitle?.toLowerCase().contains(query) ?? false);
      }).toList();

      if (_selectedSong != null &&
          !_songs.any((element) => element.id == _selectedSong!.id)) {
        _selectedSong = null;
      }
    });
  }

  void _filterAlbums() {
    final query = _albumSearchController.text.trim().toLowerCase();

    setState(() {
      _albums = _allAlbums.where((album) {
        if (query.isEmpty) return true;
        return album.title.toLowerCase().contains(query) ||
            album.artistName.toLowerCase().contains(query) ||
            (album.year?.toLowerCase().contains(query) ?? false);
      }).toList();

      if (_selectedAlbum != null &&
          !_albums.any((element) => element.id == _selectedAlbum!.id)) {
        _selectedAlbum = null;
      }
    });
  }

  Future<void> _deleteSelectedSong() async {
    if (_selectedSong == null || _isDeletingSong) return;

    final song = _selectedSong!;

    final confirmed = await ConfirmDialogs.yesNoConfirmation(
      context,
      title: "Confirmation",
      question: "Are you sure you want to delete this song?",
    );

    if (confirmed != true) return;

    setState(() {
      _isDeletingSong = true;
    });

    await Future.delayed(const Duration(milliseconds: 400));

    if (!mounted) return;

    setState(() {
      _allSongs.removeWhere((x) => x.id == song.id);
      _songs.removeWhere((x) => x.id == song.id);
      _selectedSong = null;
      _isDeletingSong = false;
    });

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text("Song '${song.title}' deleted successfully."),
      ),
    );
  }

  Future<void> _deleteSelectedAlbum() async {
    if (_selectedAlbum == null || _isDeletingAlbum) return;

    final album = _selectedAlbum!;

    final confirmed = await ConfirmDialogs.yesNoConfirmation(
      context,
      title: "Confirmation",
      question: "Are you sure you want to delete this album?",
    );

    if (confirmed != true) return;

    setState(() {
      _isDeletingAlbum = true;
    });

    await Future.delayed(const Duration(milliseconds: 400));

    if (!mounted) return;

    setState(() {
      _allAlbums.removeWhere((x) => x.id == album.id);
      _albums.removeWhere((x) => x.id == album.id);
      _selectedAlbum = null;
      _isDeletingAlbum = false;
    });

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text("Album '${album.title}' deleted successfully."),
      ),
    );
  }

  String _formatDuration(int seconds) {
    if (seconds <= 0) return '--:--';
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
            title: "Delete music",
            subtitle:
                "Delete individual songs or albums. Select an item first and then confirm the delete action.",
          ),
          const SizedBox(height: 22),
          _ModeSwitcher(
            selectedMode: _mode,
            onChanged: (value) {
              setState(() {
                _mode = value;
                _selectedSong = null;
                _selectedAlbum = null;
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
          child: _Panel(
            title: "Delete songs",
            subtitle:
                "Search songs from your system and prepare a selection for delete confirmation.",
            child: Column(
              children: [
                _SearchBox(
                  controller: _songSearchController,
                  hintText: "Search song, artist or album...",
                ),
                const SizedBox(height: 18),
                Expanded(
                  child: _songs.isEmpty
                      ? const _EmptyState(
                          title: "No songs found",
                          subtitle: "Try another search term.",
                        )
                      : ListView.separated(
                          itemCount: _songs.length,
                          separatorBuilder: (_, __) =>
                              const SizedBox(height: 12),
                          itemBuilder: (context, index) {
                            final song = _songs[index];
                            final isSelected = _selectedSong?.id == song.id;

                            return _SongDeleteCard(
                              title: song.title,
                              artist: song.artistName,
                              album: song.albumTitle ?? 'Unknown album',
                              duration: _formatDuration(song.durationSeconds),
                              coverUrl: song.coverUrl,
                              isSelected: isSelected,
                              onSelect: () {
                                setState(() {
                                  _selectedSong = song;
                                });
                              },
                            );
                          },
                        ),
                ),
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
                "Songs that will be deleted after confirmation. Review the final list here.",
            child: _selectedSong == null
                ? Column(
                    children: [
                      const Spacer(),
                      const _EmptySelectionState(
                        icon: Icons.music_note_rounded,
                        title: "No songs selected",
                        subtitle: "Select one or more songs from the left side.",
                      ),
                      const Spacer(),
                      Row(
                        children: [
                          Expanded(
                            child: OutlinedButton(
                              onPressed: null,
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
                              onPressed: null,
                              style: ElevatedButton.styleFrom(
                                minimumSize: const Size.fromHeight(46),
                                elevation: 0,
                                backgroundColor: Colors.grey.shade300,
                                foregroundColor: Colors.white,
                              ),
                              child: const Text("Delete song"),
                            ),
                          ),
                        ],
                      ),
                    ],
                  )
                : Column(
                    children: [
                      Expanded(
                        child: ListView(
                          children: [
                            _SelectedSongCard(
                              title: _selectedSong!.title,
                              artist: _selectedSong!.artistName,
                              album: _selectedSong!.albumTitle ?? 'Unknown album',
                              duration:
                                  _formatDuration(_selectedSong!.durationSeconds),
                              coverUrl: _selectedSong!.coverUrl,
                            ),
                          ],
                        ),
                      ),
                      Row(
                        children: [
                          Expanded(
                            child: OutlinedButton(
                              onPressed: () {
                                setState(() {
                                  _selectedSong = null;
                                });
                              },
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
                            child: ElevatedButton.icon(
                              onPressed:
                                  _isDeletingSong ? null : _deleteSelectedSong,
                              icon: _isDeletingSong
                                  ? const SizedBox(
                                      width: 18,
                                      height: 18,
                                      child: CircularProgressIndicator(
                                        strokeWidth: 2,
                                        color: Colors.white,
                                      ),
                                    )
                                  : const Icon(
                                      Icons.delete_outline_rounded,
                                      size: 18,
                                    ),
                              label: Text(
                                _isDeletingSong ? "Deleting..." : "Delete song",
                              ),
                              style: ElevatedButton.styleFrom(
                                minimumSize: const Size.fromHeight(46),
                                elevation: 0,
                                backgroundColor: const Color(0xFFE14D43),
                                foregroundColor: Colors.white,
                              ),
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

  Widget _buildAlbumLayout() {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Expanded(
          flex: 6,
          child: _Panel(
            title: "Delete albums",
            subtitle:
                "Search albums from your system and prepare a selection for delete confirmation.",
            child: Column(
              children: [
                _SearchBox(
                  controller: _albumSearchController,
                  hintText: "Search album, artist or year...",
                ),
                const SizedBox(height: 18),
                Expanded(
                  child: _albums.isEmpty
                      ? const _EmptyState(
                          title: "No albums found",
                          subtitle: "Try another search term.",
                        )
                      : ListView.separated(
                          itemCount: _albums.length,
                          separatorBuilder: (_, __) =>
                              const SizedBox(height: 12),
                          itemBuilder: (context, index) {
                            final album = _albums[index];
                            final isSelected = _selectedAlbum?.id == album.id;

                            return _AlbumDeleteCard(
                              title: album.title,
                              artist: album.artistName,
                              trackCountLabel: "${album.trackCount} tracks",
                              year: album.year ?? '-',
                              coverUrl: album.coverUrl,
                              isSelected: isSelected,
                              onSelect: () {
                                setState(() {
                                  _selectedAlbum = album;
                                });
                              },
                            );
                          },
                        ),
                ),
              ],
            ),
          ),
        ),
        const SizedBox(width: 18),
        Expanded(
          flex: 4,
          child: _Panel(
            title: "Selected albums",
            subtitle:
                "Albums that will be deleted after confirmation. Review the final list here.",
            child: _selectedAlbum == null
                ? Column(
                    children: [
                      const Spacer(),
                      const _EmptySelectionState(
                        icon: Icons.album_rounded,
                        title: "No albums selected",
                        subtitle: "Select one album from the left side.",
                      ),
                      const Spacer(),
                      Row(
                        children: [
                          Expanded(
                            child: OutlinedButton(
                              onPressed: null,
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
                              onPressed: null,
                              style: ElevatedButton.styleFrom(
                                minimumSize: const Size.fromHeight(46),
                                elevation: 0,
                                backgroundColor: Colors.grey.shade300,
                                foregroundColor: Colors.white,
                              ),
                              child: const Text("Delete album"),
                            ),
                          ),
                        ],
                      ),
                    ],
                  )
                : Column(
                    children: [
                      Expanded(
                        child: ListView(
                          children: [
                            _SelectedAlbumCard(
                              title: _selectedAlbum!.title,
                              artist: _selectedAlbum!.artistName,
                              year: _selectedAlbum!.year ?? '-',
                              trackCount:
                                  _selectedAlbum!.trackCount.toString(),
                              coverUrl: _selectedAlbum!.coverUrl,
                            ),
                          ],
                        ),
                      ),
                      Row(
                        children: [
                          Expanded(
                            child: OutlinedButton(
                              onPressed: () {
                                setState(() {
                                  _selectedAlbum = null;
                                });
                              },
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
                            child: ElevatedButton.icon(
                              onPressed:
                                  _isDeletingAlbum ? null : _deleteSelectedAlbum,
                              icon: _isDeletingAlbum
                                  ? const SizedBox(
                                      width: 18,
                                      height: 18,
                                      child: CircularProgressIndicator(
                                        strokeWidth: 2,
                                        color: Colors.white,
                                      ),
                                    )
                                  : const Icon(
                                      Icons.delete_outline_rounded,
                                      size: 18,
                                    ),
                              label: Text(
                                _isDeletingAlbum
                                    ? "Deleting..."
                                    : "Delete album",
                              ),
                              style: ElevatedButton.styleFrom(
                                minimumSize: const Size.fromHeight(46),
                                elevation: 0,
                                backgroundColor: const Color(0xFFE14D43),
                                foregroundColor: Colors.white,
                              ),
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
}

class _ModeSwitcher extends StatelessWidget {
  final DeleteMusicMode selectedMode;
  final ValueChanged<DeleteMusicMode> onChanged;

  const _ModeSwitcher({
    required this.selectedMode,
    required this.onChanged,
  });

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

  const _SearchBox({
    required this.controller,
    required this.hintText,
  });

  @override
  Widget build(BuildContext context) {
    return TextField(
      controller: controller,
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
    return AnimatedContainer(
      duration: const Duration(milliseconds: 160),
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: isSelected ? MusicScreen.primaryLight : Colors.white,
        border: Border.all(
          color: isSelected
              ? MusicScreen.primaryColor
              : MusicScreen.borderColor,
        ),
        borderRadius: BorderRadius.circular(12),
      ),
      child: Row(
        children: [
          _CoverImage(
            icon: Icons.music_note_rounded,
            imageUrl: coverUrl,
            size: 52,
            backgroundColor: MusicScreen.primaryLight,
            iconColor: MusicScreen.primaryColor,
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
              isSelected ? Icons.check_rounded : Icons.add_rounded,
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
  final bool isSelected;
  final VoidCallback onSelect;

  const _AlbumDeleteCard({
    required this.title,
    required this.artist,
    required this.trackCountLabel,
    required this.year,
    required this.coverUrl,
    required this.isSelected,
    required this.onSelect,
  });

  @override
  Widget build(BuildContext context) {
    return AnimatedContainer(
      duration: const Duration(milliseconds: 160),
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: isSelected ? MusicScreen.primaryLight : Colors.white,
        border: Border.all(
          color: isSelected
              ? MusicScreen.primaryColor
              : MusicScreen.borderColor,
        ),
        borderRadius: BorderRadius.circular(12),
      ),
      child: Row(
        children: [
          _CoverImage(
            icon: Icons.album_rounded,
            imageUrl: coverUrl,
            size: 62,
            backgroundColor: MusicScreen.primaryLight,
            iconColor: MusicScreen.primaryColor,
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
            onPressed: onSelect,
            icon: Icon(
              isSelected ? Icons.check_rounded : Icons.add_rounded,
              size: 18,
            ),
            label: Text(isSelected ? "Selected" : "Select"),
            style: ElevatedButton.styleFrom(
              backgroundColor: MusicScreen.primaryColor,
              foregroundColor: Colors.white,
              elevation: 0,
              minimumSize: const Size(120, 42),
            ),
          ),
        ],
      ),
    );
  }
}

class _SelectedSongCard extends StatelessWidget {
  final String title;
  final String artist;
  final String album;
  final String duration;
  final String? coverUrl;

  const _SelectedSongCard({
    required this.title,
    required this.artist,
    required this.album,
    required this.duration,
    required this.coverUrl,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(14),
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
            size: 52,
            backgroundColor: MusicScreen.primaryLight,
            iconColor: MusicScreen.primaryColor,
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
        ],
      ),
    );
  }
}

class _SelectedAlbumCard extends StatelessWidget {
  final String title;
  final String artist;
  final String year;
  final String trackCount;
  final String? coverUrl;

  const _SelectedAlbumCard({
    required this.title,
    required this.artist,
    required this.year,
    required this.trackCount,
    required this.coverUrl,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: const Color(0xFFFCFCFD),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: MusicScreen.borderColor),
      ),
      child: Row(
        children: [
          _CoverImage(
            icon: Icons.album_rounded,
            imageUrl: coverUrl,
            size: 58,
            backgroundColor: MusicScreen.primaryLight,
            iconColor: MusicScreen.primaryColor,
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
                    _InfoChip(label: "$trackCount tracks"),
                    _InfoChip(label: year),
                  ],
                ),
              ],
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

class _CoverImage extends StatelessWidget {
  final IconData icon;
  final String? imageUrl;
  final double size;
  final Color backgroundColor;
  final Color iconColor;

  const _CoverImage({
    required this.icon,
    required this.imageUrl,
    required this.size,
    required this.backgroundColor,
    required this.iconColor,
  });

  @override
  Widget build(BuildContext context) {
    final hasImage = imageUrl != null && imageUrl!.trim().isNotEmpty;

    return Container(
      width: size,
      height: size,
      decoration: BoxDecoration(
        color: backgroundColor,
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
              color: iconColor,
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
              Icons.delete_outline_rounded,
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

class _EmptySelectionState extends StatelessWidget {
  final IconData icon;
  final String title;
  final String subtitle;

  const _EmptySelectionState({
    required this.icon,
    required this.title,
    required this.subtitle,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Icon(
          icon,
          size: 54,
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
    );
  }
}