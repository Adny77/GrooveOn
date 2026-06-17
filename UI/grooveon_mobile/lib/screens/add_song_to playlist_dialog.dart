import 'package:flutter/material.dart';
import 'package:grooveon_mobile/dialogs/base_dialogs.dart';
import 'package:grooveon_mobile/helper/image_helper.dart';
import 'package:grooveon_mobile/helper/snackBar_helper.dart';
import 'package:grooveon_mobile/models/music_search_item_response.dart';
import 'package:grooveon_mobile/models/playlist_response.dart';
import 'package:grooveon_mobile/providers/playlist_provider.dart';
import 'package:grooveon_mobile/providers/playlist_song_provider.dart';
import 'package:grooveon_mobile/utils/Session.dart';
import 'package:grooveon_mobile/helper/exception_read_helper.dart';

class AddSongToPlaylistDialog extends StatefulWidget {
  final MusicSearchItemResponse song;

  const AddSongToPlaylistDialog({
    super.key,
    required this.song,
  });

  @override
  State<AddSongToPlaylistDialog> createState() =>
      _AddSongToPlaylistDialogState();
}

class _AddSongToPlaylistDialogState extends State<AddSongToPlaylistDialog> {
  static const Color primary = Color(0xFF9C27B0);
  static const Color textDark = Color(0xFF1C1C1C);

  final PlaylistProvider _playlistProvider = PlaylistProvider();
  final PlaylistSongProvider _playlistSongProvider = PlaylistSongProvider();

  bool _loading = true;
  String? _error;

  List<PlaylistResponse> _playlists = [];
  Map<int, bool> _isAddedMap = {};

  @override
  void initState() {
    super.initState();
    _loadPlaylists();
  }

  Future<void> _loadPlaylists() async {
    try {
      final userId = Session.userId;

      if (userId == null) {
        throw Exception("User is not logged in.");
      }

      final playlistResult = await _playlistProvider.get(
        filter: {
          "UserId": userId,
          "Page": 0,
          "PageSize": 100,
          "IncludeTotalCount": true,
        },
      );

      final playlistSongResult = await _playlistSongProvider.get(
        filter: {
          "SongId": widget.song.id,
          "Page": 0,
          "PageSize": 1000,
          "IncludeTotalCount": true,
        },
      );

      final existingPlaylistIds =
          playlistSongResult.items.map((e) => e.playlistId).toSet();

      if (!mounted) return;

      setState(() {
        _playlists = playlistResult.items;
        _isAddedMap = {
          for (final playlist in playlistResult.items)
            playlist.id: existingPlaylistIds.contains(playlist.id),
        };
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

  Future<void> _addToPlaylist(PlaylistResponse playlist) async {
    if (_isAddedMap[playlist.id] == true) return;

    try {
      await _playlistSongProvider.insert({
        "playlistId": playlist.id,
        "songId": widget.song.id,
      });

      if (!mounted) return;

      setState(() {
        _isAddedMap[playlist.id] = true;
      });

      SnackbarHelper.showSuccess(
        context,
        "Song added to playlist.",
      );
      Navigator.pop(context);
    } catch (e) {
      if (!mounted) return;

      SnackbarHelper.showError(
        context,
        extractErrorMessage(e),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return GrooveOnBaseDialog(
      title: "Add to playlist",
      height: 520,
      child: _content(),
    );
  }

  Widget _content() {
    if (_loading) {
      return const Center(
        child: CircularProgressIndicator(color: primary),
      );
    }

    if (_error != null) {
      return Center(
        child: Text(
          _error!,
          textAlign: TextAlign.center,
          style: const TextStyle(color: Colors.redAccent),
        ),
      );
    }

    if (_playlists.isEmpty) {
      return const Center(
        child: Text(
          "You don't have any playlists yet.",
          textAlign: TextAlign.center,
          style: TextStyle(
            color: Colors.black54,
            fontSize: 14,
            fontWeight: FontWeight.w600,
          ),
        ),
      );
    }

    return Column(
      children: [
        _songHeader(),
        const SizedBox(height: 14),
        Expanded(
          child: ListView.separated(
            itemCount: _playlists.length,
            separatorBuilder: (_, __) => const SizedBox(height: 10),
            itemBuilder: (context, index) {
              final playlist = _playlists[index];
              return _playlistTile(playlist);
            },
          ),
        ),
      ],
    );
  }

  Widget _songHeader() {
    return Row(
      children: [
        ClipRRect(
          borderRadius: BorderRadius.circular(12),
          child: widget.song.imageUrl != null &&
                  widget.song.imageUrl!.trim().isNotEmpty
              ? Image.network(
                  widget.song.imageUrl!,
                  width: 54,
                  height: 54,
                  fit: BoxFit.cover,
                )
              : Container(
                  width: 54,
                  height: 54,
                  color: primary.withOpacity(0.12),
                  child: const Icon(
                    Icons.music_note_rounded,
                    color: primary,
                  ),
                ),
        ),
        const SizedBox(width: 12),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const Text(
                "Selected song",
                style: TextStyle(
                  color: Colors.black54,
                  fontSize: 12,
                  fontWeight: FontWeight.w600,
                ),
              ),
              Text(
                widget.song.title,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                style: const TextStyle(
                  color: textDark,
                  fontSize: 15,
                  fontWeight: FontWeight.w900,
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }

  Widget _playlistTile(PlaylistResponse playlist) {
    final imageUrl = ImageHelper.playlistImageUrl(playlist.coverImageUrl);
    final isAdded = _isAddedMap[playlist.id] == true;

    return InkWell(
      borderRadius: BorderRadius.circular(14),
      onTap: isAdded ? null : () => _addToPlaylist(playlist),
      child: Container(
        padding: const EdgeInsets.all(10),
        decoration: BoxDecoration(
          color: const Color(0xFFF8F6FB),
          borderRadius: BorderRadius.circular(14),
          border: Border.all(color: const Color(0xFFE7DDF0)),
        ),
        child: Row(
          children: [
            ClipRRect(
              borderRadius: BorderRadius.circular(12),
              child: imageUrl != null
                  ? Image.network(
                      imageUrl,
                      width: 54,
                      height: 54,
                      fit: BoxFit.cover,
                    )
                  : Container(
                      width: 54,
                      height: 54,
                      color: primary.withOpacity(0.12),
                      child: const Icon(
                        Icons.queue_music_rounded,
                        color: primary,
                      ),
                    ),
            ),
            const SizedBox(width: 12),
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
                      fontSize: 15,
                      fontWeight: FontWeight.w900,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    isAdded
                        ? "Already in playlist"
                        : "${playlist.songCount} songs • ${playlist.isPublic ? "Public" : "Private"}",
                    style: TextStyle(
                      color: isAdded ? Colors.green : Colors.black54,
                      fontSize: 12,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ],
              ),
            ),
            Icon(
              isAdded ? Icons.close_rounded : Icons.add_circle_rounded,
              color: isAdded ? Colors.redAccent : primary,
              size: isAdded ? 26 : 28,
            ),
          ],
        ),
      ),
    );
  }
}