import 'dart:async';

import 'package:flutter/material.dart';
import 'package:grooveon_mobile/dialogs/confirmation_dialogs.dart';
import 'package:grooveon_mobile/helper/snackBar_helper.dart';
import 'package:grooveon_mobile/helper/universal_paging_helper.dart';
import 'package:grooveon_mobile/models/album_response.dart';
import 'package:grooveon_mobile/models/music_search_item_response.dart';
import 'package:grooveon_mobile/models/search_results.dart';
import 'package:grooveon_mobile/providers/album_provider.dart';
import 'package:grooveon_mobile/providers/music_search_provider.dart';
import 'package:grooveon_mobile/providers/player_provider.dart';
import 'package:grooveon_mobile/providers/user_provider.dart';
import 'package:grooveon_mobile/screens/add_song_to%20playlist_dialog.dart';
import 'package:grooveon_mobile/screens/artist_info_screen.dart';
import 'package:grooveon_mobile/screens/universal_album_preview_screen.dart';
import 'package:grooveon_mobile/utils/Session.dart';
import 'package:grooveon_mobile/widgets/mini_player_bar.dart';
import 'package:grooveon_mobile/widgets/swipe_widget.dart';
import 'package:provider/provider.dart';

class SearchScreen extends StatefulWidget {
  const SearchScreen({super.key});

  @override
  State<SearchScreen> createState() => _SearchScreenState();
}

class _SearchScreenState extends State<SearchScreen> {
  static const Color primaryDark = Color(0xFF4A148C);
  static const Color lightPurple = Color(0xFFF3E5F5);
  static const Color textDark = Color(0xFF1C1C1C);
  static const Color pageBg = Color(0xFFF8F6FB);

  final TextEditingController _controller = TextEditingController();
  Timer? _debounce;

  late final UniversalPagingProvider<MusicSearchItemResponse> _pagingProvider;
  late final UserProvider _userProvider;

  @override
  void initState() {
    super.initState();

    _userProvider = context.read<UserProvider>();

    _pagingProvider = UniversalPagingProvider<MusicSearchItemResponse>(
      pageSize: 10,
      fetcher: ({
        required int page,
        required int pageSize,
        String? filter,
        Map<String, dynamic>? extra,
        bool includeTotalCount = true,
      }) async {
        final musicSearchProvider = context.read<MusicSearchProvider>();

        final result = await musicSearchProvider.search(
          fts: filter,
          page: page,
          pageSize: pageSize,
          includeTotalCount: includeTotalCount,
          retrieveAll: false,
        );

        final sorted = _sortResults(result.items.toList(), filter ?? "");

        return SearchResult<MusicSearchItemResponse>(
          items: sorted,
          totalCount: result.totalCount ?? 0,
        );
      },
    );
  }

  static List<MusicSearchItemResponse> _sortResults(
    List<MusicSearchItemResponse> items,
    String query,
  ) {
    final q = query.trim().toLowerCase();

    if (q.isEmpty) return items;

    int score(MusicSearchItemResponse e) {
      final title = e.title.toLowerCase();
      final subtitle = (e.subtitle ?? '').toLowerCase();
      final type = e.type.toLowerCase();

      int s = 0;

      if (title == q) s += 100;
      if (title.startsWith(q)) s += 60;
      if (title.contains(q)) s += 30;

      if (subtitle == q) s += 35;
      if (subtitle.startsWith(q)) s += 20;
      if (subtitle.contains(q)) s += 10;

      if (type == 'artist') s += 40;
      if (type == 'song') s += 10;

      return s;
    }

    items.sort((a, b) {
      final byScore = score(b).compareTo(score(a));
      if (byScore != 0) return byScore;
      return a.title.toLowerCase().compareTo(b.title.toLowerCase());
    });

    return items;
  }

  @override
  void dispose() {
    _debounce?.cancel();
    _controller.dispose();
    _pagingProvider.dispose();
    super.dispose();
  }

  void _onChanged(String value) {
    _debounce?.cancel();

    _debounce = Timer(const Duration(milliseconds: 350), () async {
      final query = value.trim();

      if (query.isEmpty) {
        _pagingProvider.clear();
        if (mounted) setState(() {});
        return;
      }

      await _pagingProvider.search(query);

      if (mounted) setState(() {});
    });

    if (mounted) {
      setState(() {});
    }
  }

  void _clearSearch() {
    _debounce?.cancel();
    _controller.clear();
    _pagingProvider.clear();
    setState(() {});
  }

  Future<void> _openArtist(MusicSearchItemResponse item) async {
    try {
      final hasPremium = await _userProvider.hasPremium();

      if (!hasPremium) {
        if (!mounted) return;

        await ConfirmDialogs.okConfirmation(
          context,
          title: "Premium Required",
          message:
              "A premium subscription is required to access artist profiles.\n\nActivate premium to continue.",
          okText: "OK",
          danger: true,
        );

        return;
      }

      if (!mounted) return;

      Navigator.push(
        context,
        MaterialPageRoute(
          builder: (_) => ArtistInfoScreen(
            artistId: item.artistId ?? item.id,
            artistName: item.title,
            artistImageUrl: item.imageUrl,
            description: null,
            playCount: 0,
          ),
        ),
      );
    } catch (e) {
      if (!mounted) return;
      SnackbarHelper.showError(context, e.toString());
    }
  }

  Future<void> _openAlbum(MusicSearchItemResponse album) async {
  try {
    final hasPremium = await _userProvider.hasPremium();

    if (!hasPremium) {
      if (!mounted) return;

      await ConfirmDialogs.okConfirmation(
        context,
        title: "Premium Required",
        message:
            "A premium subscription is required to access albums.\n\nActivate premium to continue.",
        okText: "OK",
        danger: true,
      );

      return;
    }

    if (!mounted) return;

    final albumProvider = context.read<AlbumProvider>();

    final albumResponse = await albumProvider.getById(album.id);

    if (!mounted) return;

    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (_) => UniversalAlbumPreviewScreen(
          album: albumResponse,
        ),
      ),
    );
  } catch (e) {
    if (!mounted) return;

    SnackbarHelper.showError(context, e.toString());
  }
}

  Future<void> _openAddToPlaylistDialog(MusicSearchItemResponse song) async {
    await showDialog<bool>(
      context: context,
      builder: (_) => AddSongToPlaylistDialog(song: song),
    );
  }

  Future<void> _playSong(MusicSearchItemResponse song) async {
    try {
      final extId = song.externalTrackId;

      if (extId == null || extId.trim().isEmpty) {
        throw Exception("Song does not have an externalTrackId.");
      }

      final player = context.read<PlayerProvider>();

      await player.playSongWithPurpose(
        request: {
          "userId": Session.userId,
          "songId": song.id,
          "purpose": "RandomMusic",
        },
      );
    } catch (e) {
      if (!mounted) return;

      SnackbarHelper.showError(context, e.toString());
    }
  }

  @override
  Widget build(BuildContext context) {
    final query = _controller.text.trim();

    return Scaffold(
      backgroundColor: pageBg,
      body: Stack(
        children: [
          SafeArea(
            child: Column(
              children: [
                _buildTopBar(),
                const SizedBox(height: 12),
                Expanded(
                  child: query.isEmpty
                      ? _buildIdleState()
                      : SingleChildScrollView(
                          padding: const EdgeInsets.fromLTRB(16, 0, 16, 90),
                          child: SwipePagedList<MusicSearchItemResponse>(
                            provider: _pagingProvider,
                            separatorHeight: 10,
                            itemBuilder: (context, item) => _buildItem(item),
                          ),
                        ),
                ),
              ],
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

  Widget _buildTopBar() {
    return Padding(
      padding: const EdgeInsets.fromLTRB(16, 12, 16, 0),
      child: Row(
        children: [
          IconButton(
            onPressed: () => Navigator.pop(context),
            icon: const Icon(Icons.arrow_back_ios_new_rounded),
          ),
          Expanded(
            child: Container(
              height: 48,
              decoration: BoxDecoration(
                color: Colors.white,
                borderRadius: BorderRadius.circular(22),
                boxShadow: [
                  BoxShadow(
                    color: Colors.black.withOpacity(0.05),
                    blurRadius: 10,
                  ),
                ],
              ),
              child: TextField(
                controller: _controller,
                autofocus: true,
                textAlignVertical: TextAlignVertical.center,
                onChanged: _onChanged,
                decoration: InputDecoration(
                  hintText: "Search songs, artists, albums...",
                  hintStyle: const TextStyle(
                    color: Colors.black54,
                    fontSize: 14,
                  ),
                  border: InputBorder.none,
                  isDense: true,
                  contentPadding: const EdgeInsets.symmetric(vertical: 12),
                  prefixIcon: const Icon(
                    Icons.search,
                    size: 20,
                    color: Colors.black54,
                  ),
                  prefixIconConstraints: const BoxConstraints(
                    minHeight: 20,
                    minWidth: 40,
                  ),
                  suffixIcon: _controller.text.isNotEmpty
                      ? IconButton(
                          onPressed: _clearSearch,
                          icon: const Icon(Icons.close, size: 20),
                        )
                      : null,
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildIdleState() {
    return const Center(
      child: Padding(
        padding: EdgeInsets.symmetric(horizontal: 28),
        child: Text(
          "Start typing to search songs, artists and albums.",
          textAlign: TextAlign.center,
          style: TextStyle(
            color: Colors.black54,
            fontSize: 15,
            fontWeight: FontWeight.w500,
          ),
        ),
      ),
    );
  }

  Widget _buildItem(MusicSearchItemResponse item) {
    switch (item.type.toLowerCase()) {
      case 'artist':
        return _artistTile(item);
      case 'album':
        return _albumTile(item);
      case 'song':
      default:
        return _songTile(item);
    }
  }

  Widget _artistTile(MusicSearchItemResponse artist) {
    return InkWell(
      onTap: () => _openArtist(artist),
      borderRadius: BorderRadius.circular(16),
      child: Container(
        padding: const EdgeInsets.all(10),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(16),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withOpacity(0.04),
              blurRadius: 10,
              offset: const Offset(0, 4),
            ),
          ],
        ),
        child: Row(
          children: [
            _imageBox(
              imageUrl: artist.imageUrl,
              size: 58,
              isCircle: true,
              fallbackIcon: Icons.person,
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    artist.title,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: const TextStyle(
                      fontSize: 14,
                      fontWeight: FontWeight.w700,
                      color: textDark,
                    ),
                  ),
                  const SizedBox(height: 4),
                  const Text(
                    "Artist",
                    style: TextStyle(
                      fontSize: 12,
                      color: Colors.black54,
                      fontWeight: FontWeight.w500,
                    ),
                  ),
                ],
              ),
            ),
            const Icon(Icons.arrow_forward_ios_rounded, size: 16),
          ],
        ),
      ),
    );
  }

  Widget _songTile(MusicSearchItemResponse song) {
    final subtitle =
        song.subtitle?.trim().isNotEmpty == true ? song.subtitle! : "Song";

    return InkWell(
      onTap: () => _playSong(song),
      borderRadius: BorderRadius.circular(16),
      child: Container(
        padding: const EdgeInsets.all(10),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(16),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withOpacity(0.04),
              blurRadius: 10,
              offset: const Offset(0, 4),
            ),
          ],
        ),
        child: Row(
          children: [
            _imageBox(
              imageUrl: song.imageUrl,
              size: 58,
              isCircle: false,
              fallbackIcon: Icons.music_note_rounded,
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
                      fontSize: 14,
                      fontWeight: FontWeight.w700,
                      color: textDark,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    subtitle,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: const TextStyle(
                      fontSize: 12,
                      color: Colors.black54,
                      fontWeight: FontWeight.w500,
                    ),
                  ),
                ],
              ),
            ),
            Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                InkWell(
                  borderRadius: BorderRadius.circular(20),
                  onTap: () {
                    _openAddToPlaylistDialog(song);
                  },
                  child: Container(
                    width: 38,
                    height: 38,
                    decoration: const BoxDecoration(
                      color: lightPurple,
                      shape: BoxShape.circle,
                    ),
                    child: const Icon(
                      Icons.add_rounded,
                      color: primaryDark,
                    ),
                  ),
                ),
                const SizedBox(width: 8),
                InkWell(
                  borderRadius: BorderRadius.circular(20),
                  onTap: () => _playSong(song),
                  child: Container(
                    width: 38,
                    height: 38,
                    decoration: const BoxDecoration(
                      color: lightPurple,
                      shape: BoxShape.circle,
                    ),
                    child: const Icon(
                      Icons.play_arrow_rounded,
                      color: primaryDark,
                    ),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _albumTile(MusicSearchItemResponse album) {
    final subtitle =
        album.subtitle?.trim().isNotEmpty == true ? album.subtitle! : "Album";

    return InkWell(
      onTap: () => _openAlbum(album),
      borderRadius: BorderRadius.circular(16),
      child: Container(
        padding: const EdgeInsets.all(10),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(16),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withOpacity(0.04),
              blurRadius: 10,
              offset: const Offset(0, 4),
            ),
          ],
        ),
        child: Row(
          children: [
            _imageBox(
              imageUrl: album.imageUrl,
              size: 58,
              isCircle: false,
              fallbackIcon: Icons.album,
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
                      fontSize: 14,
                      fontWeight: FontWeight.w700,
                      color: textDark,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    subtitle,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: const TextStyle(
                      fontSize: 12,
                      color: Colors.black54,
                      fontWeight: FontWeight.w500,
                    ),
                  ),
                ],
              ),
            ),
            const Icon(
              Icons.arrow_forward_ios_rounded,
              size: 16,
              color: Colors.black45,
            ),
          ],
        ),
      ),
    );
  }

  Widget _imageBox({
    required String? imageUrl,
    required double size,
    required bool isCircle,
    required IconData fallbackIcon,
  }) {
    return Container(
      width: size,
      height: size,
      decoration: BoxDecoration(
        color: Colors.grey.shade300,
        shape: isCircle ? BoxShape.circle : BoxShape.rectangle,
        borderRadius: isCircle ? null : BorderRadius.circular(14),
        image: imageUrl != null && imageUrl.trim().isNotEmpty
            ? DecorationImage(
                image: NetworkImage(imageUrl),
                fit: BoxFit.cover,
              )
            : null,
      ),
      child: imageUrl == null || imageUrl.trim().isEmpty
          ? Icon(fallbackIcon, color: Colors.white)
          : null,
    );
  }
}