import 'dart:async';

import 'package:flutter/material.dart';
import 'package:grooveon_mobile/dialogs/confirmation_dialogs.dart';
import 'package:grooveon_mobile/helper/image_helper.dart';
import 'package:grooveon_mobile/helper/snackBar_helper.dart';
import 'package:grooveon_mobile/models/playlist_response.dart';
import 'package:grooveon_mobile/providers/playlist_provider.dart';
import 'package:grooveon_mobile/providers/user_provider.dart';
import 'package:grooveon_mobile/screens/create_playlist_screen.dart';
import 'package:grooveon_mobile/screens/universal_playlist_preview_screen.dart';
import 'package:grooveon_mobile/utils/Session.dart';

class MyPlaylistsScreen extends StatefulWidget {
  const MyPlaylistsScreen({super.key});

  @override
  State<MyPlaylistsScreen> createState() => _MyPlaylistsScreenState();
}

class _MyPlaylistsScreenState extends State<MyPlaylistsScreen> {
  static const Color primary = Color(0xFF9C27B0);
  static const Color bg = Color(0xFFF8F6FB);
  static const Color textDark = Color(0xFF1C1C1C);

  final PlaylistProvider _playlistProvider = PlaylistProvider();
  final UserProvider _userProvider = UserProvider();
  final TextEditingController _searchController = TextEditingController();

  bool _isLoading = true;
  String? _error;
  List<PlaylistResponse> _playlists = [];
  Timer? _searchDebounce;

  @override
  void initState() {
    super.initState();
    _loadPlaylists();
  }

  Future<void> _deletePlaylist(PlaylistResponse playlist) async {
    final confirmed = await ConfirmDialogs.yesNoConfirmation(
      context,
      title: "Delete playlist",
      question:
          "Are you sure you want to delete '${playlist.name}' and all songs inside it?",
      yesText: "Delete",
      noText: "Cancel",
      danger: true,
    );

    if (!confirmed) return;

    try {
      await _playlistProvider.delete(playlist.id);

      if (!mounted) return;

      SnackbarHelper.showSuccess(context, "Playlist deleted successfully.");

      await _loadPlaylists();
    } catch (e) {
      if (!mounted) return;

      SnackbarHelper.showError(context, e.toString());
    }
  }

  Future<void> _loadPlaylists() async {
    try {
      setState(() {
        _isLoading = true;
        _error = null;
      });

      final userId = Session.userId;

      if (userId == null) {
        throw Exception("User is not logged in.");
      }

      final result = await _playlistProvider.get(
        filter: {
          "UserId": userId,
          "Page": 0,
          "PageSize": 50,
          "IncludeTotalCount": true,
          if (_searchController.text.trim().isNotEmpty)
            "FTS": _searchController.text.trim(),
        },
      );

      if (!mounted) return;

      setState(() {
        _playlists = result.items;
        _isLoading = false;
      });
    } catch (e) {
      if (!mounted) return;

      setState(() {
        _error = e.toString();
        _isLoading = false;
      });
    }
  }

  void _onSearchChanged(String value) {
    setState(() {});
    _searchDebounce?.cancel();
    _searchDebounce = Timer(const Duration(milliseconds: 350), () {
      _loadPlaylists();
    });
  }

  void _clearSearch() {
    if (_searchController.text.isEmpty) return;

    _searchController.clear();
    setState(() {});
    _loadPlaylists();
  }

  @override
  void dispose() {
    _searchDebounce?.cancel();
    _searchController.dispose();
    super.dispose();
  }

  Future<void> _openCreatePlaylist() async {
    try {
      final hasPremium = await _userProvider.hasPremium();

      final playlistCount = hasPremium
          ? _playlists.length
          : await _getCurrentUserPlaylistCount();

      if (!hasPremium && playlistCount >= 3) {
        await ConfirmDialogs.premiumLockedDialog(
          context,
          title: "Playlist limit",
          message:
              "Basic users can create up to 3 playlists.\n\nActivate premium to create more playlists.",
          okText: "OK",
        );

        return;
      }

      final created = await Navigator.push<bool>(
        context,
        MaterialPageRoute(builder: (_) => const CreatePlaylistScreen()),
      );

      if (created == true) {
        await _loadPlaylists();

        if (!mounted) return;

        SnackbarHelper.showSuccess(context, "Playlist created successfully.");
      }
    } catch (e) {
      if (!mounted) return;

      SnackbarHelper.showError(context, e.toString());
    }
  }

  Future<int> _getCurrentUserPlaylistCount() async {
    final userId = Session.userId;

    if (userId == null) {
      throw Exception("User is not logged in.");
    }

    final result = await _playlistProvider.get(
      filter: {
        "UserId": userId,
        "Page": 0,
        "PageSize": 1,
        "IncludeTotalCount": true,
      },
    );

    return result.totalCount;
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: bg,
      body: SafeArea(
        child: RefreshIndicator(
          onRefresh: _loadPlaylists,
          color: primary,
          child: ListView(
            physics: const AlwaysScrollableScrollPhysics(),
            padding: const EdgeInsets.fromLTRB(18, 18, 18, 100),
            children: [
              _header(),
              const SizedBox(height: 20),
              _searchBar(),
              const SizedBox(height: 16),
              if (_isLoading)
                const Padding(
                  padding: EdgeInsets.only(top: 120),
                  child: Center(
                    child: CircularProgressIndicator(color: primary),
                  ),
                )
              else if (_error != null)
                _errorCard()
              else if (_playlists.isEmpty)
                _emptyCard()
              else
                ..._playlists.map(_playlistCard),
            ],
          ),
        ),
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
            color: Colors.black.withOpacity(0.04),
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
              onChanged: _onSearchChanged,
              textInputAction: TextInputAction.search,
              decoration: const InputDecoration(
                hintText: "Search my playlists",
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
              tooltip: "Clear search",
            ),
        ],
      ),
    );
  }

  Widget _header() {
    return Row(
      children: [
        const Expanded(
          child: Text(
            "My playlists",
            style: TextStyle(
              color: textDark,
              fontSize: 28,
              fontWeight: FontWeight.w900,
            ),
          ),
        ),
        ElevatedButton.icon(
          onPressed: _openCreatePlaylist,
          icon: const Icon(Icons.add_rounded, size: 18),
          label: const Text("Create playlist"),
          style: ElevatedButton.styleFrom(
            backgroundColor: primary,
            foregroundColor: Colors.white,
            elevation: 0,
            padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 11),
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(18),
            ),
          ),
        ),
      ],
    );
  }

  Widget _playlistCard(PlaylistResponse playlist) {
    final imageUrl = ImageHelper.playlistImageUrl(playlist.coverImageUrl);

    return InkWell(
      borderRadius: BorderRadius.circular(22),
      onTap: () {
        Navigator.push(
          context,
          MaterialPageRoute(
            builder: (_) => UniversalPlaylistPreviewScreen(playlist: playlist),
          ),
        );
      },
      child: Container(
        margin: const EdgeInsets.only(bottom: 14),
        padding: const EdgeInsets.all(12),
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
              width: 68,
              height: 68,
              decoration: BoxDecoration(
                color: primary.withOpacity(0.12),
                borderRadius: BorderRadius.circular(16),
                image: imageUrl != null
                    ? DecorationImage(
                        image: NetworkImage(imageUrl),
                        fit: BoxFit.cover,
                      )
                    : null,
              ),
              child: imageUrl == null
                  ? const Icon(
                      Icons.queue_music_rounded,
                      color: primary,
                      size: 32,
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
                      fontSize: 17,
                      fontWeight: FontWeight.w900,
                    ),
                  ),
                  const SizedBox(height: 5),
                  Text(
                    playlist.description?.trim().isNotEmpty == true
                        ? playlist.description!
                        : "No description",
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                    style: const TextStyle(
                      color: Colors.black54,
                      fontSize: 13,
                      fontWeight: FontWeight.w500,
                    ),
                  ),
                  const SizedBox(height: 6),
                  Text(
                    "${playlist.songCount} songs • ${playlist.isPublic ? "Public" : "Private"}",
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
            InkWell(
              borderRadius: BorderRadius.circular(20),
              onTap: () => _deletePlaylist(playlist),
              child: const SizedBox(
                width: 38,
                height: 38,
                child: Icon(
                  Icons.delete_outline_rounded,
                  color: Colors.redAccent,
                  size: 25,
                ),
              ),
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
      ),
      child: const Column(
        children: [
          Icon(Icons.grid_view_rounded, color: primary, size: 52),
          SizedBox(height: 12),
          Text(
            "No playlists yet",
            style: TextStyle(
              color: textDark,
              fontSize: 18,
              fontWeight: FontWeight.w900,
            ),
          ),
          SizedBox(height: 6),
          Text(
            "Create your first playlist and add your favorite songs.",
            textAlign: TextAlign.center,
            style: TextStyle(color: Colors.black54, fontSize: 13),
          ),
        ],
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
          const Icon(
            Icons.error_outline_rounded,
            color: Colors.redAccent,
            size: 44,
          ),
          const SizedBox(height: 10),
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
            style: const TextStyle(color: Colors.black54, fontSize: 13),
          ),
        ],
      ),
    );
  }
}
