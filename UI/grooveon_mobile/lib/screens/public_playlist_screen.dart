import 'package:flutter/material.dart';
import 'package:grooveon_mobile/helper/image_helper.dart';
import 'package:grooveon_mobile/helper/snackBar_helper.dart';
import 'package:grooveon_mobile/helper/universal_paging_helper.dart';
import 'package:grooveon_mobile/models/playlist_response.dart';
import 'package:grooveon_mobile/providers/playlist_provider.dart';
import 'package:grooveon_mobile/screens/universal_playlist_preview_screen.dart';
import 'package:grooveon_mobile/utils/Session.dart';
import 'package:grooveon_mobile/widgets/swipe_widget.dart';

class PublicPlaylistsScreen extends StatefulWidget {
  const PublicPlaylistsScreen({super.key});

  @override
  State<PublicPlaylistsScreen> createState() => _PublicPlaylistsScreenState();
}

class _PublicPlaylistsScreenState extends State<PublicPlaylistsScreen> {
  static const Color primary = Color(0xFF9C27B0);
  static const Color bg = Color(0xFFF8F6FB);
  static const Color textDark = Color(0xFF1C1C1C);
  static const Color textMuted = Color(0xFF7A7A85);

  final PlaylistProvider _playlistProvider = PlaylistProvider();
  late final UniversalPagingProvider<PlaylistResponse> _paging;

  bool _loading = true;
  String? _error;

  @override
  void initState() {
    super.initState();

    _paging = UniversalPagingProvider<PlaylistResponse>(
      pageSize: 5,
      fetcher: ({
        required int page,
        required int pageSize,
        String? filter,
        Map<String, dynamic>? extra,
        bool includeTotalCount = true,
      }) async {
        final query = <String, dynamic>{
          "ExcludeUserId": Session.userId,
          "Page": page,
          "PageSize": pageSize,
          "IncludeTotalCount": includeTotalCount,
          ...?extra,
        };

        if (filter != null && filter.trim().isNotEmpty) {
          query["FTS"] = filter.trim();
        }

        return await _playlistProvider.get(filter: query);
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

      await _paging.loadPage(pageNumber: 0);

      if (!mounted) return;

      setState(() => _loading = false);
    } catch (e) {
      if (!mounted) return;

      setState(() {
        _error = e.toString();
        _loading = false;
      });
    }
  }

  Future<void> _refresh() async {
    try {
      await _paging.refresh();
      if (mounted) setState(() {});
    } catch (e) {
      if (!mounted) return;
      SnackbarHelper.showError(context, e.toString());
    }
  }

  void _openPlaylist(PlaylistResponse playlist) {
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (_) => UniversalPlaylistPreviewScreen(
          playlist: playlist,
        ),
      ),
    );
  }

  @override
  void dispose() {
    _paging.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: bg,
      body: SafeArea(
        child: Padding(
          padding: const EdgeInsets.fromLTRB(18, 18, 18, 100),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const Text(
                "Public playlists",
                style: TextStyle(
                  color: textDark,
                  fontSize: 26,
                  fontWeight: FontWeight.w900,
                ),
              ),
              const SizedBox(height: 6),
              const Text(
                "Discover playlists created by other GrooveOn users.",
                style: TextStyle(
                  color: textMuted,
                  fontSize: 13,
                  fontWeight: FontWeight.w600,
                ),
              ),
              const SizedBox(height: 20),
              Expanded(
                child: _loading
                    ? const Center(
                        child: CircularProgressIndicator(color: primary),
                      )
                    : _error != null
                        ? _errorCard()
                        : RefreshIndicator(
                            onRefresh: _refresh,
                            color: primary,
                            child: _paging.items.isEmpty
                                ? ListView(
                                    physics:
                                        const AlwaysScrollableScrollPhysics(),
                                    children: [
                                      _emptyCard(),
                                    ],
                                  )
                                :  SingleChildScrollView(
    physics: const AlwaysScrollableScrollPhysics(),
    child: SwipePagedList<PlaylistResponse>(
      provider: _paging,
      separatorHeight: 14,
      itemBuilder: (context, playlist) {
        return _playlistCard(playlist);
      },
    ),
  ),
                          ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _playlistCard(PlaylistResponse playlist) {
    final imageUrl = ImageHelper.playlistImageUrl(playlist.coverImageUrl);

    return InkWell(
      onTap: () => _openPlaylist(playlist),
      borderRadius: BorderRadius.circular(22),
      child: Container(
        padding: const EdgeInsets.all(14),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(22),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withOpacity(0.04),
              blurRadius: 14,
              offset: const Offset(0, 5),
            ),
          ],
        ),
        child: Row(
          children: [
            Container(
              width: 66,
              height: 66,
              decoration: BoxDecoration(
                color: const Color(0xFFF1ECF7),
                borderRadius: BorderRadius.circular(16),
                image: imageUrl != null && imageUrl.trim().isNotEmpty
                    ? DecorationImage(
                        image: NetworkImage(imageUrl),
                        fit: BoxFit.cover,
                      )
                    : null,
              ),
              child: imageUrl == null || imageUrl.trim().isEmpty
                  ? const Icon(
                      Icons.queue_music_rounded,
                      color: primary,
                      size: 30,
                    )
                  : null,
            ),
            const SizedBox(width: 14),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    playlist.name,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: const TextStyle(
                      color: textDark,
                      fontSize: 16,
                      fontWeight: FontWeight.w900,
                    ),
                  ),
                  const SizedBox(height: 5),
                  Text(
                    playlist.description?.trim().isNotEmpty == true
                        ? playlist.description!
                        : "Public GrooveOn playlist",
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                    style: const TextStyle(
                      color: textMuted,
                      fontSize: 12.5,
                      height: 1.25,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                  const SizedBox(height: 8),
                  Text(
                    "${playlist.songCount} songs",
                    style: const TextStyle(
                      color: primary,
                      fontSize: 12,
                      fontWeight: FontWeight.w800,
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(width: 8),
            const Icon(
              Icons.chevron_right_rounded,
              color: primary,
              size: 30,
            ),
          ],
        ),
      ),
    );
  }

  Widget _emptyCard() {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(22),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(22),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.04),
            blurRadius: 14,
            offset: const Offset(0, 5),
          ),
        ],
      ),
      child: const Column(
        children: [
          Icon(
            Icons.queue_music_rounded,
            color: primary,
            size: 52,
          ),
          SizedBox(height: 12),
          Text(
            "No public playlists",
            style: TextStyle(
              color: textDark,
              fontSize: 18,
              fontWeight: FontWeight.w900,
            ),
          ),
          SizedBox(height: 6),
          Text(
            "Public playlists from other users will appear here.",
            textAlign: TextAlign.center,
            style: TextStyle(
              color: Colors.black54,
              fontSize: 13,
              fontWeight: FontWeight.w500,
            ),
          ),
        ],
      ),
    );
  }

  Widget _errorCard() {
    return Center(
      child: Container(
        width: double.infinity,
        padding: const EdgeInsets.all(22),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(22),
        ),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Icon(
              Icons.error_outline_rounded,
              color: primary,
              size: 42,
            ),
            const SizedBox(height: 12),
            const Text(
              "Failed to load playlists",
              style: TextStyle(
                color: textDark,
                fontSize: 18,
                fontWeight: FontWeight.w900,
              ),
            ),
            const SizedBox(height: 8),
            Text(
              _error ?? "Unknown error",
              textAlign: TextAlign.center,
              style: const TextStyle(
                color: textMuted,
                fontSize: 13,
              ),
            ),
            const SizedBox(height: 16),
            ElevatedButton(
              onPressed: _loadData,
              style: ElevatedButton.styleFrom(
                backgroundColor: primary,
                foregroundColor: Colors.white,
              ),
              child: const Text("Try again"),
            ),
          ],
        ),
      ),
    );
  }
}