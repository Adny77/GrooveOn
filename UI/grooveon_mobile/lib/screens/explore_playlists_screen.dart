import 'package:flutter/material.dart';
import 'package:grooveon_mobile/helper/image_helper.dart';
import 'package:grooveon_mobile/helper/universal_paging_helper.dart';
import 'package:grooveon_mobile/models/playlist_response.dart';
import 'package:grooveon_mobile/providers/playlist_provider.dart';
import 'package:grooveon_mobile/screens/universal_playlist_preview_screen.dart';
import 'package:grooveon_mobile/utils/Session.dart';
import 'package:grooveon_mobile/widgets/swipe_widget.dart';
import 'package:grooveon_mobile/helper/exception_read_helper.dart';

class ExplorePlaylistsScreen extends StatefulWidget {
  const ExplorePlaylistsScreen({super.key});

  @override
  State<ExplorePlaylistsScreen> createState() => _ExplorePlaylistsScreenState();
}

class _ExplorePlaylistsScreenState extends State<ExplorePlaylistsScreen> {
  static const Color primary = Color(0xFF9C27B0);
  static const Color bg = Color(0xFFF8F6FB);
  static const Color textDark = Color(0xFF1C1C1C);

  final PlaylistProvider _provider = PlaylistProvider();
  final TextEditingController _searchController = TextEditingController();

  late final UniversalPagingProvider<PlaylistResponse> _paging;

  bool _initialLoading = true;
  String? _error;

  @override
  void initState() {
    super.initState();

    _paging = UniversalPagingProvider<PlaylistResponse>(
      pageSize: 10,
      fetcher: ({
        required int page,
        required int pageSize,
        String? filter,
        Map<String, dynamic>? extra,
        bool includeTotalCount = true,
      }) async {
        return await _provider.get(filter: {
          'IsPublic': true,
          if (Session.userId != null) 'ExcludeUserId': Session.userId,
          'Page': page,
          'PageSize': pageSize,
          'IncludeTotalCount': includeTotalCount,
          if (filter != null && filter.trim().isNotEmpty) 'FTS': filter.trim(),
        });
      },
    );

    _load();
  }

  @override
  void dispose() {
    _paging.dispose();
    _searchController.dispose();
    super.dispose();
  }

  Future<void> _load() async {
    setState(() {
      _initialLoading = true;
      _error = null;
    });
    try {
      await _paging.loadPage(pageNumber: 0);
      if (!mounted) return;
      setState(() => _initialLoading = false);
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _error = extractErrorMessage(e);
        _initialLoading = false;
      });
    }
  }

  Future<void> _search(String value) async {
    await _paging.search(value);
    if (mounted) setState(() {});
  }

  void _clearSearch() {
    if (_searchController.text.isEmpty) return;
    _searchController.clear();
    _search('');
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: bg,
      body: SafeArea(
        child: RefreshIndicator(
          onRefresh: _load,
          color: primary,
          child: ListView(
            physics: const AlwaysScrollableScrollPhysics(),
            padding: const EdgeInsets.fromLTRB(18, 18, 18, 100),
            children: [
              _header(),
              const SizedBox(height: 20),
              _searchBar(),
              const SizedBox(height: 16),
              if (_initialLoading)
                const Padding(
                  padding: EdgeInsets.only(top: 120),
                  child: Center(child: CircularProgressIndicator(color: primary)),
                )
              else if (_error != null)
                _errorCard()
              else
                ListenableBuilder(
                  listenable: _paging,
                  builder: (context, _) => Column(
                    children: [
                      SwipePagedList<PlaylistResponse>(
                        provider: _paging,
                        separatorHeight: 14,
                        showHint: false,
                        itemBuilder: (context, playlist) =>
                            _playlistCard(playlist),
                      ),
                      if (_paging.totalPages > 1) ...[
                        const SizedBox(height: 16),
                        _pageControls(),
                      ],
                    ],
                  ),
                ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _header() {
    return const Text(
      'Explore playlists',
      style: TextStyle(
        color: textDark,
        fontSize: 28,
        fontWeight: FontWeight.w900,
      ),
    );
  }

  Widget _searchBar() {
    return Container(
      height: 52,
      padding: const EdgeInsets.symmetric(horizontal: 14),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(18),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.04),
            blurRadius: 14,
            offset: const Offset(0, 5),
          ),
        ],
      ),
      child: Row(
        children: [
          const Icon(Icons.search_rounded, color: primary, size: 23),
          const SizedBox(width: 10),
          Expanded(
            child: TextField(
              controller: _searchController,
              onChanged: _search,
              textInputAction: TextInputAction.search,
              decoration: const InputDecoration(
                hintText: 'Search public playlists',
                border: InputBorder.none,
                isCollapsed: true,
              ),
              style: const TextStyle(
                color: textDark,
                fontSize: 14,
                fontWeight: FontWeight.w700,
              ),
            ),
          ),
          if (_searchController.text.isNotEmpty)
            IconButton(
              onPressed: _clearSearch,
              icon: const Icon(Icons.close_rounded, color: Colors.black45),
              tooltip: 'Clear search',
            ),
        ],
      ),
    );
  }

  Widget _pageControls() {
    return Row(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        IconButton(
          onPressed: _paging.hasPreviousPage
              ? () async {
                  await _paging.previousPage();
                  if (mounted) setState(() {});
                }
              : null,
          icon: Icon(
            Icons.arrow_back_ios_rounded,
            size: 20,
            color: _paging.hasPreviousPage ? primary : Colors.black26,
          ),
        ),
        Container(
          padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 6),
          decoration: BoxDecoration(
            color: primary.withValues(alpha: 0.10),
            borderRadius: BorderRadius.circular(20),
          ),
          child: Text(
            '${_paging.page + 1} / ${_paging.totalPages}',
            style: const TextStyle(
              color: primary,
              fontWeight: FontWeight.w800,
              fontSize: 14,
            ),
          ),
        ),
        IconButton(
          onPressed: _paging.hasNextPage
              ? () async {
                  await _paging.nextPage();
                  if (mounted) setState(() {});
                }
              : null,
          icon: Icon(
            Icons.arrow_forward_ios_rounded,
            size: 20,
            color: _paging.hasNextPage ? primary : Colors.black26,
          ),
        ),
      ],
    );
  }

  Widget _playlistCard(PlaylistResponse playlist) {
    final imageUrl = ImageHelper.playlistImageUrl(playlist.coverImageUrl);

    return InkWell(
      borderRadius: BorderRadius.circular(22),
      onTap: () => Navigator.push(
        context,
        MaterialPageRoute(
          builder: (_) => UniversalPlaylistPreviewScreen(
            playlist: playlist,
            canRemoveSongs: false,
          ),
        ),
      ),
      child: Container(
        padding: const EdgeInsets.all(12),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(22),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withValues(alpha: 0.04),
              blurRadius: 14,
              offset: const Offset(0, 5),
            ),
          ],
        ),
        child: Row(
          children: [
            Container(
              width: 68,
              height: 68,
              decoration: BoxDecoration(
                color: primary.withValues(alpha: 0.12),
                borderRadius: BorderRadius.circular(16),
                image: imageUrl != null
                    ? DecorationImage(
                        image: NetworkImage(imageUrl),
                        fit: BoxFit.cover,
                      )
                    : null,
              ),
              child: imageUrl == null
                  ? const Icon(Icons.queue_music_rounded, color: primary, size: 32)
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
                      fontSize: 17,
                      fontWeight: FontWeight.w900,
                    ),
                  ),
                  const SizedBox(height: 4),
                  if (playlist.username != null)
                    Text(
                      'by ${playlist.username}',
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                      style: const TextStyle(
                        color: Colors.black45,
                        fontSize: 12,
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                  const SizedBox(height: 4),
                  Text(
                    '${playlist.songCount} songs',
                    style: const TextStyle(
                      color: primary,
                      fontSize: 12,
                      fontWeight: FontWeight.w800,
                    ),
                  ),
                ],
              ),
            ),
            const Icon(Icons.chevron_right_rounded, color: Colors.black26),
          ],
        ),
      ),
    );
  }

  Widget _errorCard() {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(22),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(22),
      ),
      child: Column(
        children: [
          const Icon(Icons.error_outline_rounded, color: Colors.redAccent, size: 44),
          const SizedBox(height: 10),
          const Text(
            'Failed to load playlists',
            style: TextStyle(color: textDark, fontSize: 18, fontWeight: FontWeight.w900),
          ),
          const SizedBox(height: 8),
          Text(
            _error ?? 'Unknown error',
            textAlign: TextAlign.center,
            style: const TextStyle(color: Colors.black54, fontSize: 13),
          ),
        ],
      ),
    );
  }
}
