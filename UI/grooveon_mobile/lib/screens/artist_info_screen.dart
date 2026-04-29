import 'package:flutter/material.dart';
import 'package:grooveon_mobile/helper/snackBar_helper.dart';
import 'package:grooveon_mobile/screens/universal_album_preview_screen.dart';
import 'package:provider/provider.dart';

import 'package:grooveon_mobile/helper/universal_paging_helper.dart';
import 'package:grooveon_mobile/models/album_response.dart';
import 'package:grooveon_mobile/models/song_response.dart';
import 'package:grooveon_mobile/providers/album_provider.dart';
import 'package:grooveon_mobile/providers/song_provider.dart';
import 'package:grooveon_mobile/providers/player_provider.dart';
import 'package:grooveon_mobile/widgets/swipe_widget.dart';
import 'package:grooveon_mobile/widgets/mini_player_bar.dart';
import 'package:grooveon_mobile/utils/Session.dart';

class ArtistInfoScreen extends StatefulWidget {
  final int artistId;
  final String artistName;
  final String? artistImageUrl;
  final String? description;
  final int playCount;

  const ArtistInfoScreen({
    super.key,
    required this.artistId,
    required this.artistName,
    this.artistImageUrl,
    this.description,
    required this.playCount,
  });

  @override
  State<ArtistInfoScreen> createState() => _ArtistInfoScreenState();
}

class _ArtistInfoScreenState extends State<ArtistInfoScreen>
    with SingleTickerProviderStateMixin {
  static const Color groovePurple = Color(0xFF9C27B0);
  static const Color groovePurpleDark = Color(0xFF4A148C);

  static const Color bg = Color(0xFFF8F6FB);
  static const Color card = Color(0xFFFFFFFF);
  static const Color card2 = Color(0xFFF1ECF7);

  static const Color textPrimary = Color(0xFF1C1C1C);
  static const Color textSecondary = Color(0xFF7A7A85);
  static const Color divider = Color(0xFFE6E6EF);

  late final TabController _tabController;

  final AlbumProvider _albumProvider = AlbumProvider();
  final SongProvider _songProvider = SongProvider();

  late final UniversalPagingProvider<AlbumResponse> _albumsPaging;
  late final UniversalPagingProvider<SongResponse> _songsPaging;

  bool _headerLoading = true;
  String? _headerError;
  int _albumCount = 0;
  int _songCount = 0;

  @override
  void initState() {
    super.initState();

    _tabController = TabController(length: 2, vsync: this);

    _albumsPaging = UniversalPagingProvider<AlbumResponse>(
      pageSize: 6,
      fetcher: ({
        required int page,
        required int pageSize,
        String? filter,
        Map<String, dynamic>? extra,
        bool includeTotalCount = true,
      }) async {
        final query = <String, dynamic>{
          "ArtistId": widget.artistId,
          "Page": page,
          "PageSize": pageSize,
          "IncludeTotalCount": includeTotalCount,
          ...?extra,
        };

        if (filter != null && filter.trim().isNotEmpty) {
          query["FTS"] = filter.trim();
        }

        return await _albumProvider.get(filter: query);
      },
    );

    _songsPaging = UniversalPagingProvider<SongResponse>(
      pageSize: 6,
      fetcher: ({
        required int page,
        required int pageSize,
        String? filter,
        Map<String, dynamic>? extra,
        bool includeTotalCount = true,
      }) async {
        final query = <String, dynamic>{
          "ArtistId": widget.artistId,
          "Page": page,
          "PageSize": pageSize,
          "IncludeTotalCount": includeTotalCount,
          ...?extra,
        };

        if (filter != null && filter.trim().isNotEmpty) {
          query["FTS"] = filter.trim();
        }

        return await _songProvider.get(filter: query);
      },
    );

    _loadInitialData();
  }

  Future<void> _loadInitialData() async {
    try {
      setState(() {
        _headerLoading = true;
        _headerError = null;
      });

      await Future.wait([
        _albumsPaging.loadPage(pageNumber: 0),
        _songsPaging.loadPage(pageNumber: 0),
      ]);

      if (!mounted) return;

      setState(() {
        _albumCount = _albumsPaging.totalCount;
        _songCount = _songsPaging.totalCount;
        _headerLoading = false;
      });
    } catch (e) {
      if (!mounted) return;

      setState(() {
        _headerError = e.toString();
        _headerLoading = false;
      });
    }
  }

  Future<void> _refreshCurrentTab() async {
    if (_tabController.index == 0) {
      await _songsPaging.refresh();
    } else {
      await _albumsPaging.refresh();
    }

    if (!mounted) return;

    setState(() {
      _albumCount = _albumsPaging.totalCount;
      _songCount = _songsPaging.totalCount;
    });
  }

  Future<void> _playRandomArtistSong(SongResponse song) async {
    try {
      final userId = Session.userId;

      if (userId == null) {
        throw Exception("User is not signed in.");
      }

      await context.read<PlayerProvider>().playSongWithPurpose(
            request: {
              "userId": userId,
              "songId": song.id,
              "purpose": "RandomMusicArtist",
              "artistId": widget.artistId,
            },
          );
    } catch (e) {
      debugPrint("RANDOM ARTIST PLAY ERROR: $e");

      if (!context.mounted) return;

      SnackbarHelper.showInfo(context, "Preview is currently unavailable for this song.");
    }
  }

  Future<void> _startRandomArtistMusic() async {
    final songs = _songsPaging.items;

    if (songs.isEmpty) {
      SnackbarHelper.showInfo(context, "This artist currently has no songs.");
      return;
    }

    await _playRandomArtistSong(songs.first);
  }

  @override
  void dispose() {
    _tabController.dispose();
    _albumsPaging.dispose();
    _songsPaging.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: bg,
      body: Stack(
        children: [
          _headerLoading
              ? _loadingState()
              : _headerError != null
                  ? _errorState()
                  : DefaultTabController(
                      length: 2,
                      child: NestedScrollView(
                        headerSliverBuilder: (context, innerBoxIsScrolled) {
                          return [
                            SliverAppBar(
                              automaticallyImplyLeading: false,
                              backgroundColor: bg,
                              expandedHeight: 420,
                              pinned: true,
                              elevation: 0,
                              flexibleSpace: FlexibleSpaceBar(
                                background: Stack(
                                  fit: StackFit.expand,
                                  children: [
                                    _heroImage(),
                                    Container(
                                      decoration: BoxDecoration(
                                        gradient: LinearGradient(
                                          begin: Alignment.topCenter,
                                          end: Alignment.bottomCenter,
                                          colors: [
                                            Colors.black.withOpacity(0.05),
                                            Colors.black.withOpacity(0.14),
                                            Colors.black.withOpacity(0.40),
                                            bg,
                                          ],
                                          stops: const [0, 0.25, 0.65, 1],
                                        ),
                                      ),
                                    ),
                                    SafeArea(
                                      child: Column(
                                        children: [
                                          Padding(
                                            padding: const EdgeInsets.fromLTRB(
                                              14,
                                              10,
                                              14,
                                              0,
                                            ),
                                            child: Row(
                                              children: [
                                                _circleIconButton(
                                                  icon:
                                                      Icons.arrow_back_ios_new,
                                                  onTap: () =>
                                                      Navigator.pop(context),
                                                ),
                                              ],
                                            ),
                                          ),
                                          const Spacer(),
                                          Padding(
                                            padding: const EdgeInsets.fromLTRB(
                                              18,
                                              0,
                                              18,
                                              18,
                                            ),
                                            child: Column(
                                              crossAxisAlignment:
                                                  CrossAxisAlignment.start,
                                              children: [
                                                Text(
                                                  widget.artistName,
                                                  maxLines: 2,
                                                  overflow:
                                                      TextOverflow.ellipsis,
                                                  style: const TextStyle(
                                                    color: Colors.white,
                                                    fontSize: 34,
                                                    fontWeight: FontWeight.w900,
                                                    height: 1.02,
                                                  ),
                                                ),
                                                const SizedBox(height: 8),
                                                Text(
                                                  "${_formatCount(widget.playCount)} total plays • $_songCount songs • $_albumCount albums",
                                                  style: const TextStyle(
                                                    color: Colors.white70,
                                                    fontSize: 14,
                                                    fontWeight: FontWeight.w600,
                                                  ),
                                                ),
                                                if (widget.description != null &&
                                                    widget.description!
                                                        .trim()
                                                        .isNotEmpty) ...[
                                                  const SizedBox(height: 10),
                                                  Text(
                                                    widget.description!,
                                                    maxLines: 2,
                                                    overflow:
                                                        TextOverflow.ellipsis,
                                                    style: const TextStyle(
                                                      color: Colors.white70,
                                                      fontSize: 13,
                                                      height: 1.35,
                                                      fontWeight:
                                                          FontWeight.w500,
                                                    ),
                                                  ),
                                                ],
                                                const SizedBox(height: 18),
                                                _actionRow(),
                                                const SizedBox(height: 18),
                                                _tabRow(),
                                              ],
                                            ),
                                          ),
                                        ],
                                      ),
                                    ),
                                  ],
                                ),
                              ),
                            ),
                          ];
                        },
                        body: RefreshIndicator(
                          onRefresh: _refreshCurrentTab,
                          color: groovePurple,
                          child: TabBarView(
                            controller: _tabController,
                            children: [
                              _musicTab(),
                              _albumsTab(),
                            ],
                          ),
                        ),
                      ),
                    ),
          const Align(
            alignment: Alignment.bottomCenter,
            child: MiniPlayerBar(),
          ),
        ],
      ),
    );
  }

  Widget _heroImage() {
    if (widget.artistImageUrl != null &&
        widget.artistImageUrl!.trim().isNotEmpty) {
      return Image.network(
        widget.artistImageUrl!,
        fit: BoxFit.cover,
        errorBuilder: (_, __, ___) => _fallbackHero(),
      );
    }

    return _fallbackHero();
  }

  Widget _fallbackHero() {
    return Container(
      decoration: const BoxDecoration(
        gradient: LinearGradient(
          colors: [
            groovePurpleDark,
            groovePurple,
            Color(0xFFE9D8F1),
          ],
          begin: Alignment.topCenter,
          end: Alignment.bottomCenter,
        ),
      ),
      child: const Center(
        child: Icon(
          Icons.person_rounded,
          color: Colors.white70,
          size: 100,
        ),
      ),
    );
  }

  Widget _circleIconButton({
    required IconData icon,
    required VoidCallback onTap,
  }) {
    return Material(
      color: Colors.white.withOpacity(0.18),
      shape: const CircleBorder(),
      child: InkWell(
        customBorder: const CircleBorder(),
        onTap: onTap,
        child: SizedBox(
          width: 42,
          height: 42,
          child: Icon(
            icon,
            color: Colors.white,
            size: 20,
          ),
        ),
      ),
    );
  }

  Widget _actionRow() {
    return Row(
      children: [
        Container(
          width: 54,
          height: 54,
          decoration: BoxDecoration(
            color: card,
            borderRadius: BorderRadius.circular(12),
            border: Border.all(color: divider),
            boxShadow: [
              BoxShadow(
                color: groovePurple.withOpacity(0.08),
                blurRadius: 10,
                offset: const Offset(0, 4),
              ),
            ],
          ),
          child: widget.artistImageUrl != null &&
                  widget.artistImageUrl!.trim().isNotEmpty
              ? ClipRRect(
                  borderRadius: BorderRadius.circular(12),
                  child: Image.network(
                    widget.artistImageUrl!,
                    fit: BoxFit.cover,
                  ),
                )
              : const Icon(
                  Icons.music_note_rounded,
                  color: groovePurple,
                ),
        ),
        const Spacer(),
        InkWell(
          onTap: _startRandomArtistMusic,
          borderRadius: BorderRadius.circular(31),
          child: Container(
            width: 62,
            height: 62,
            decoration: const BoxDecoration(
              color: groovePurple,
              shape: BoxShape.circle,
              boxShadow: [
                BoxShadow(
                  color: Color(0x559C27B0),
                  blurRadius: 18,
                  offset: Offset(0, 8),
                ),
              ],
            ),
            child: const Icon(
              Icons.play_arrow_rounded,
              color: Colors.white,
              size: 34,
            ),
          ),
        ),
      ],
    );
  }

  Widget _tabRow() {
    return Container(
      alignment: Alignment.centerLeft,
      child: TabBar(
        controller: _tabController,
        isScrollable: true,
        tabAlignment: TabAlignment.start,
        dividerColor: Colors.transparent,
        indicatorColor: groovePurple,
        indicatorWeight: 3,
        labelColor: Colors.white,
        unselectedLabelColor: Colors.white70,
        labelStyle: const TextStyle(
          fontSize: 17,
          fontWeight: FontWeight.w900,
        ),
        unselectedLabelStyle: const TextStyle(
          fontSize: 17,
          fontWeight: FontWeight.w700,
        ),
        tabs: const [
          Tab(text: "Music"),
          Tab(text: "Albums"),
        ],
      ),
    );
  }

  Widget _musicTab() {
    return ListView(
      physics: const AlwaysScrollableScrollPhysics(),
      padding: const EdgeInsets.fromLTRB(18, 18, 18, 90),
      children: [
        Text(
          "Popular ($_songCount)",
          style: const TextStyle(
            color: textPrimary,
            fontSize: 18,
            fontWeight: FontWeight.w900,
          ),
        ),
        const SizedBox(height: 14),
        SwipePagedList<SongResponse>(
          provider: _songsPaging,
          separatorHeight: 14,
          itemBuilder: (context, song) {
            final index = _songsPaging.items.indexOf(song);
            return _buildSongRow(song, index + 1);
          },
        ),
      ],
    );
  }

  Widget _albumsTab() {
    return ListView(
      physics: const AlwaysScrollableScrollPhysics(),
      padding: const EdgeInsets.fromLTRB(18, 18, 18, 90),
      children: [
        Text(
          "Albums ($_albumCount)",
          style: const TextStyle(
            color: textPrimary,
            fontSize: 18,
            fontWeight: FontWeight.w900,
          ),
        ),
        const SizedBox(height: 14),
        SwipePagedList<AlbumResponse>(
          provider: _albumsPaging,
          separatorHeight: 14,
          itemBuilder: (context, album) => _buildAlbumRow(album),
        ),
      ],
    );
  }

  Widget _buildSongRow(SongResponse song, int number) {
    return InkWell(
      borderRadius: BorderRadius.circular(12),
      onTap: () => _playRandomArtistSong(song),
      child: Container(
        padding: const EdgeInsets.symmetric(vertical: 4),
        child: Row(
          children: [
            SizedBox(
              width: 28,
              child: Text(
                "$number",
                style: const TextStyle(
                  color: textSecondary,
                  fontSize: 17,
                  fontWeight: FontWeight.w700,
                ),
              ),
            ),
            const SizedBox(width: 10),
            Container(
              width: 58,
              height: 58,
              decoration: BoxDecoration(
                color: card2,
                borderRadius: BorderRadius.circular(8),
                image: song.coverUrl != null && song.coverUrl!.trim().isNotEmpty
                    ? DecorationImage(
                        image: NetworkImage(song.coverUrl!),
                        fit: BoxFit.cover,
                      )
                    : null,
                boxShadow: [
                  BoxShadow(
                    color: groovePurple.withOpacity(0.05),
                    blurRadius: 8,
                    offset: const Offset(0, 3),
                  ),
                ],
              ),
              child: song.coverUrl == null || song.coverUrl!.trim().isEmpty
                  ? const Icon(
                      Icons.music_note_rounded,
                      color: groovePurple,
                    )
                  : null,
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    song.title,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: const TextStyle(
                      color: groovePurple,
                      fontSize: 16,
                      fontWeight: FontWeight.w800,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    _songSubtitle(song),
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: const TextStyle(
                      color: textSecondary,
                      fontSize: 13,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(width: 8),
            const Icon(
              Icons.play_circle_fill_rounded,
              color: groovePurple,
              size: 28,
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildAlbumRow(AlbumResponse album) {
    final year = album.releaseDate?.year;

    return InkWell(
      borderRadius: BorderRadius.circular(16),
      onTap: () {
        Navigator.push(
          context,
          MaterialPageRoute(
            builder: (_) => UniversalAlbumPreviewScreen(album: album),
          ),
        );
      },
      child: Container(
        padding: const EdgeInsets.all(10),
        decoration: BoxDecoration(
          color: card,
          borderRadius: BorderRadius.circular(16),
          border: Border.all(color: divider),
          boxShadow: [
            BoxShadow(
              color: groovePurple.withOpacity(0.06),
              blurRadius: 10,
              offset: const Offset(0, 4),
            ),
          ],
        ),
        child: Row(
          children: [
            Container(
              width: 72,
              height: 72,
              decoration: BoxDecoration(
                color: card2,
                borderRadius: BorderRadius.circular(12),
                image: album.coverUrl != null &&
                        album.coverUrl!.trim().isNotEmpty
                    ? DecorationImage(
                        image: NetworkImage(album.coverUrl!),
                        fit: BoxFit.cover,
                      )
                    : null,
              ),
              child: album.coverUrl == null || album.coverUrl!.trim().isEmpty
                  ? const Icon(
                      Icons.album_rounded,
                      color: groovePurple,
                      size: 30,
                    )
                  : null,
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    album.title,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: const TextStyle(
                      color: groovePurple,
                      fontSize: 16,
                      fontWeight: FontWeight.w800,
                    ),
                  ),
                  const SizedBox(height: 6),
                  Text(
                    year != null
                        ? "$year • ${album.songCount} songs"
                        : "${album.songCount} songs",
                    style: const TextStyle(
                      color: textSecondary,
                      fontSize: 13,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                  if (album.description != null &&
                      album.description!.trim().isNotEmpty) ...[
                    const SizedBox(height: 6),
                    Text(
                      album.description!,
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                      style: const TextStyle(
                        color: textSecondary,
                        fontSize: 12.5,
                        height: 1.3,
                      ),
                    ),
                  ],
                ],
              ),
            ),
            const SizedBox(width: 8),
            const Icon(
              Icons.chevron_right_rounded,
              color: textSecondary,
              size: 24,
            ),
          ],
        ),
      ),
    );
  }

  String _songSubtitle(SongResponse song) {
    final duration = _formatDuration(song.durationSeconds);

    if (song.albumTitle != null && song.albumTitle!.trim().isNotEmpty) {
      return "$duration • ${song.albumTitle}";
    }

    return duration;
  }

  String _formatDuration(int seconds) {
    final minutes = seconds ~/ 60;
    final remainingSeconds = seconds % 60;
    return "$minutes:${remainingSeconds.toString().padLeft(2, '0')}";
  }

  String _formatCount(int value) {
    if (value >= 1000000) {
      return "${(value / 1000000).toStringAsFixed(1)}M";
    }

    if (value >= 1000) {
      return "${(value / 1000).toStringAsFixed(1)}K";
    }

    return "$value";
  }

  Widget _loadingState() {
    return Container(
      color: bg,
      child: const Center(
        child: CircularProgressIndicator(color: groovePurple),
      ),
    );
  }

  Widget _errorState() {
    return Container(
      color: bg,
      padding: const EdgeInsets.all(24),
      child: Center(
        child: Container(
          padding: const EdgeInsets.all(22),
          decoration: BoxDecoration(
            color: card,
            borderRadius: BorderRadius.circular(22),
            border: Border.all(color: divider),
          ),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              const Icon(
                Icons.error_outline_rounded,
                color: Colors.redAccent,
                size: 40,
              ),
              const SizedBox(height: 12),
              const Text(
                "Failed to load artist data",
                style: TextStyle(
                  color: textPrimary,
                  fontSize: 18,
                  fontWeight: FontWeight.w900,
                ),
              ),
              const SizedBox(height: 8),
              Text(
                _headerError ?? "Unknown error",
                textAlign: TextAlign.center,
                style: const TextStyle(
                  color: textSecondary,
                  fontSize: 13,
                ),
              ),
              const SizedBox(height: 16),
              ElevatedButton(
                onPressed: _loadInitialData,
                style: ElevatedButton.styleFrom(
                  backgroundColor: groovePurple,
                  foregroundColor: Colors.white,
                ),
                child: const Text("Try Again"),
              ),
            ],
          ),
        ),
      ),
    );
  }
}