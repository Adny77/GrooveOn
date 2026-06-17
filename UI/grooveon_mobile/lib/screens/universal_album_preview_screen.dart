import 'package:flutter/material.dart';
import 'package:grooveon_mobile/helper/exception_read_helper.dart';
import 'package:grooveon_mobile/helper/snackBar_helper.dart';
import 'package:provider/provider.dart';

import 'package:grooveon_mobile/helper/universal_paging_helper.dart';
import 'package:grooveon_mobile/models/album_response.dart';
import 'package:grooveon_mobile/models/song_response.dart';
import 'package:grooveon_mobile/providers/song_provider.dart';
import 'package:grooveon_mobile/providers/player_provider.dart';
import 'package:grooveon_mobile/widgets/swipe_widget.dart';
import 'package:grooveon_mobile/widgets/mini_player_bar.dart';
import 'package:grooveon_mobile/utils/Session.dart';

class UniversalAlbumPreviewScreen extends StatefulWidget {
  final AlbumResponse album;

  const UniversalAlbumPreviewScreen({
    super.key,
    required this.album,
  });

  @override
  State<UniversalAlbumPreviewScreen> createState() =>
      _UniversalAlbumPreviewScreenState();
}

class _UniversalAlbumPreviewScreenState
    extends State<UniversalAlbumPreviewScreen> {
  static const Color groovePurple = Color(0xFF9C27B0);
  static const Color groovePurpleDark = Color(0xFF4A148C);

  static const Color bg = Color(0xFFF8F6FB);
  static const Color card = Color(0xFFFFFFFF);
  static const Color card2 = Color(0xFFF1ECF7);

  static const Color textPrimary = Color(0xFF1C1C1C);
  static const Color textSecondary = Color(0xFF7A7A85);
  static const Color divider = Color(0xFFE6E6EF);

  final SongProvider _songProvider = SongProvider();
  late final UniversalPagingProvider<SongResponse> _songsPaging;

  bool _loading = true;
  String? _error;
  int _songCount = 0;

  @override
  void initState() {
    super.initState();

    _songsPaging = UniversalPagingProvider<SongResponse>(
      pageSize: 20,
      fetcher: ({
        required int page,
        required int pageSize,
        String? filter,
        Map<String, dynamic>? extra,
        bool includeTotalCount = true,
      }) async {
        final query = <String, dynamic>{
          "AlbumId": widget.album.id,
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

    _loadData();
  }

  Future<void> _loadData() async {
    try {
      setState(() {
        _loading = true;
        _error = null;
      });

      await _songsPaging.loadPage(pageNumber: 0);

      if (!mounted) return;

      setState(() {
        _songCount = _songsPaging.totalCount;
        _loading = false;
      });
    } catch (e) {
      if (!mounted) return;

      setState(() {
        _error = extractErrorMessage(e);
        _loading = false;
      });
    }
  }

  Future<void> _refresh() async {
    await _songsPaging.refresh();

    if (!mounted) return;

    setState(() {
      _songCount = _songsPaging.totalCount;
    });
  }

  Future<void> _playSong(SongResponse song) async {
    try {
      final userId = Session.userId;

      if (userId == null) {
  throw Exception("User is not logged in.");
}

      await context.read<PlayerProvider>().playSongWithPurpose(
            request: {
              "userId": userId,
              "songId": song.id,
              "purpose": "AlbumList",
              "artistId": widget.album.artistId,
            },
          );
    } catch (e) {
      if (!context.mounted) return;

      SnackbarHelper.showInfo(context, "Preview is currently not available for this song.");
    }
  }

  Future<void> _startAlbumFromFirstSong() async {
    final items = _songsPaging.items;

    if (items.isEmpty) {
        SnackbarHelper.showInfo(context, "This album has no songs.");

      return;
    }

    await _playSong(items.first);
  }

  @override
  void dispose() {
    _songsPaging.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: bg,
      body: Stack(
        children: [
          _loading
              ? _loadingState()
              : _error != null
                  ? _errorState()
                  : NestedScrollView(
                      headerSliverBuilder: (context, innerBoxIsScrolled) {
                        return [
                          SliverAppBar(
                            automaticallyImplyLeading: false,
                            backgroundColor: bg,
                            expandedHeight: 410,
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
                                                icon: Icons.arrow_back_ios_new_rounded,
                                                onTap: () => Navigator.pop(context),
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
                                              Container(
                                                padding:
                                                    const EdgeInsets.symmetric(
                                                  horizontal: 12,
                                                  vertical: 6,
                                                ),
                                                decoration: BoxDecoration(
                                                  color:
                                                      Colors.white.withOpacity(0.18),
                                                  borderRadius:
                                                      BorderRadius.circular(30),
                                                  border: Border.all(
                                                    color: Colors.white
                                                        .withOpacity(0.18),
                                                  ),
                                                ),
                                                child: const Text(
                                                  "ALBUM",
                                                  style: TextStyle(
                                                    color: Colors.white,
                                                    fontSize: 11,
                                                    fontWeight: FontWeight.w800,
                                                    letterSpacing: 1.1,
                                                  ),
                                                ),
                                              ),
                                              const SizedBox(height: 12),
                                              Text(
                                                widget.album.title,
                                                maxLines: 2,
                                                overflow: TextOverflow.ellipsis,
                                                style: const TextStyle(
                                                  color: Colors.white,
                                                  fontSize: 34,
                                                  fontWeight: FontWeight.w900,
                                                  height: 1.02,
                                                ),
                                              ),
                                              const SizedBox(height: 8),
                                              Text(
                                                _metaText(),
                                                style: const TextStyle(
                                                  color: Colors.white70,
                                                  fontSize: 14,
                                                  fontWeight: FontWeight.w600,
                                                ),
                                              ),
                                              if (widget.album.description != null &&
                                                  widget.album.description!
                                                      .trim()
                                                      .isNotEmpty) ...[
                                                const SizedBox(height: 12),
                                                Text(
                                                  widget.album.description!,
                                                  maxLines: 3,
                                                  overflow: TextOverflow.ellipsis,
                                                  style: const TextStyle(
                                                    color: Colors.white70,
                                                    fontSize: 13,
                                                    height: 1.35,
                                                    fontWeight: FontWeight.w500,
                                                  ),
                                                ),
                                              ],
                                              const SizedBox(height: 18),
                                              _actionRow(),
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
                        onRefresh: _refresh,
                        color: groovePurple,
                        child: ListView(
                          physics: const AlwaysScrollableScrollPhysics(),
                          padding: const EdgeInsets.fromLTRB(18, 18, 18, 90),
                          children: [
                            Row(
                              children: [
                                Text(
                                  "Tracks ($_songCount)",
                                  style: const TextStyle(
                                    color: textPrimary,
                                    fontSize: 18,
                                    fontWeight: FontWeight.w900,
                                  ),
                                ),
                                const Spacer(),
                                Container(
                                  padding: const EdgeInsets.symmetric(
                                    horizontal: 10,
                                    vertical: 6,
                                  ),
                                  decoration: BoxDecoration(
                                    color: card,
                                    borderRadius: BorderRadius.circular(30),
                                    border: Border.all(color: divider),
                                  ),
                                  child: Text(
                                    "${_songCount} songs",
                                    style: const TextStyle(
                                      color: textSecondary,
                                      fontSize: 12,
                                      fontWeight: FontWeight.w700,
                                    ),
                                  ),
                                ),
                              ],
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
    if (widget.album.coverUrl != null &&
        widget.album.coverUrl!.trim().isNotEmpty) {
      return Image.network(
        widget.album.coverUrl!,
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
          Icons.album_rounded,
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
          child: widget.album.coverUrl != null &&
                  widget.album.coverUrl!.trim().isNotEmpty
              ? ClipRRect(
                  borderRadius: BorderRadius.circular(12),
                  child: Image.network(
                    widget.album.coverUrl!,
                    fit: BoxFit.cover,
                  ),
                )
              : const Icon(
                  Icons.album_rounded,
                  color: groovePurple,
                ),
        ),
        const Spacer(),
        InkWell(
          onTap: _startAlbumFromFirstSong,
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

  Widget _buildSongRow(SongResponse song, int number) {
    return InkWell(
      borderRadius: BorderRadius.circular(12),
      onTap: () => _playSong(song),
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

  String _songSubtitle(SongResponse song) {
    final duration = _formatDuration(song.durationSeconds);

    if (song.artistName != null && song.artistName!.trim().isNotEmpty) {
      return "$duration • ${song.artistName}";
    }

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

  String _metaText() {
    final parts = <String>[];

    if (widget.album.artistName.trim().isNotEmpty) {
      parts.add(widget.album.artistName);
    }

    if (widget.album.releaseDate != null) {
      parts.add(widget.album.releaseDate!.year.toString());
    }

    parts.add("${_songCount} songs");

    return parts.join(" • ");
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
                "Failed to load album",
                style: TextStyle(
                  color: textPrimary,
                  fontSize: 18,
                  fontWeight: FontWeight.w900,
                ),
              ),
              const SizedBox(height: 8),
              Text(
                _error ?? "Unknown error",
                textAlign: TextAlign.center,
                style: const TextStyle(
                  color: textSecondary,
                  fontSize: 13,
                ),
              ),
              const SizedBox(height: 16),
              ElevatedButton(
                onPressed: _loadData,
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