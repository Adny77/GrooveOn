import 'package:file_picker/file_picker.dart';
import 'package:flutter/material.dart';
import 'package:grooveon_mobile/helper/snackBar_helper.dart';
import 'package:provider/provider.dart';
import 'package:http/http.dart' as http;

import '../providers/player_provider.dart';
import '../deezer/provider/deezer_track_provider.dart';
import 'package:grooveon_mobile/helper/exception_read_helper.dart';

class MiniPlayerBar extends StatefulWidget {
  const MiniPlayerBar({super.key});

  @override
  State<MiniPlayerBar> createState() => _MiniPlayerBarState();
}

class _MiniPlayerBarState extends State<MiniPlayerBar> {
  static const Color purple = Color(0xFF9C27B0);
  static const Color purpleDark = Color(0xFF4A148C);

  static const double minSize = 0.11;
  static const double maxSize = 0.92;

  double _sheetSize = minSize;

  double get _openProgress {
    return ((_sheetSize - minSize) / (maxSize - minSize)).clamp(0.0, 1.0);
  }

  double get _miniOpacity {
    return (1.0 - (_openProgress * 1.8)).clamp(0.0, 1.0);
  }

  @override
  Widget build(BuildContext context) {
    return Consumer<PlayerProvider>(
      builder: (context, player, child) {
        if (!player.hasSong) return const SizedBox.shrink();

        return NotificationListener<DraggableScrollableNotification>(
          onNotification: (notification) {
            if (mounted) {
              setState(() {
                _sheetSize = notification.extent;
              });
            }
            return false;
          },
          child: DraggableScrollableSheet(
            initialChildSize: minSize,
            minChildSize: minSize,
            maxChildSize: maxSize,
            snap: true,
            snapSizes: const [minSize, maxSize],
            builder: (context, scrollController) {
              return Container(
                decoration: const BoxDecoration(
                  gradient: LinearGradient(
                    colors: [purple, purpleDark],
                    begin: Alignment.topCenter,
                    end: Alignment.bottomCenter,
                  ),
                  borderRadius: BorderRadius.vertical(top: Radius.circular(28)),
                ),
                child: SingleChildScrollView(
                  controller: scrollController,
                  physics: const ClampingScrollPhysics(),
                  child: Column(
                    children: [
                      const SizedBox(height: 8),

                      Container(
                        width: 40,
                        height: 4,
                        decoration: BoxDecoration(
                          color: Colors.white54,
                          borderRadius: BorderRadius.circular(10),
                        ),
                      ),

                      const SizedBox(height: 10),

                      Opacity(
                        opacity: _miniOpacity,
                        child: IgnorePointer(
                          ignoring: _miniOpacity < 0.15,
                          child: _MiniContent(player: player),
                        ),
                      ),

                      SizedBox(height: 20 * _miniOpacity),

                      _FullContent(player: player),

                      const SizedBox(height: 30),
                    ],
                  ),
                ),
              );
            },
          ),
        );
      },
    );
  }
}

class _MiniContent extends StatelessWidget {
  final PlayerProvider player;

  const _MiniContent({required this.player});

  @override
  Widget build(BuildContext context) {
    final song = player.currentSong;

    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16),
      child: Row(
        children: [
          _CoverArt(
            url: song?.coverUrl,
            width: 50,
            height: 50,
            borderRadius: 12,
            iconSize: 24,
          ),
          const SizedBox(width: 12),

          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  song?.title ?? "",
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: const TextStyle(
                    color: Colors.white,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                Text(
                  song?.artistName ?? "GrooveOn",
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: const TextStyle(color: Colors.white70, fontSize: 12),
                ),
              ],
            ),
          ),

          Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              _miniBtn(
                icon: Icons.skip_previous_rounded,
                size: 28,
                color: player.hasPrevious ? Colors.white : Colors.white30,
                onPressed: player.hasPrevious ? player.playPrevious : null,
              ),
              _miniPlayBtn(player),
              _miniBtn(
                icon: Icons.skip_next_rounded,
                size: 28,
                color: player.hasNext ? Colors.white : Colors.white30,
                onPressed: player.hasNext ? player.playNext : null,
              ),
              _miniBtn(
                icon: Icons.close_rounded,
                size: 22,
                color: Colors.white,
                onPressed: player.stopPlayer,
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _miniBtn({
    required IconData icon,
    required double size,
    required Color color,
    required VoidCallback? onPressed,
  }) {
    return SizedBox(
      width: 36,
      height: 36,
      child: IconButton(
        padding: EdgeInsets.zero,
        onPressed: onPressed,
        icon: Icon(icon, color: color, size: size),
      ),
    );
  }

  Widget _miniPlayBtn(PlayerProvider player) {
    return SizedBox(
      width: 36,
      height: 36,
      child: IconButton(
        padding: EdgeInsets.zero,
        onPressed: player.isLoading
            ? null
            : () {
                if (player.isCompleted) {
                  player.repeatCurrentSong();
                } else {
                  player.togglePlayPause();
                }
              },
        icon: player.isLoading
            ? const SizedBox(
                width: 20,
                height: 20,
                child: CircularProgressIndicator(
                  strokeWidth: 2,
                  color: Colors.white,
                ),
              )
            : Icon(
                player.isCompleted
                    ? Icons.repeat_rounded
                    : player.isPlaying
                        ? Icons.pause_rounded
                        : Icons.play_arrow_rounded,
                color: Colors.white,
                size: 30,
              ),
      ),
    );
  }
}

class _CoverArt extends StatelessWidget {
  final String? url;
  final double width;
  final double height;
  final double borderRadius;
  final double iconSize;

  const _CoverArt({
    required this.url,
    required this.width,
    required this.height,
    required this.borderRadius,
    required this.iconSize,
  });

  @override
  Widget build(BuildContext context) {
    final imageUrl = url?.trim();

    return ClipRRect(
      borderRadius: BorderRadius.circular(borderRadius),
      child: SizedBox(
        width: width,
        height: height,
        child: imageUrl != null && imageUrl.isNotEmpty
            ? Image.network(
                imageUrl,
                fit: BoxFit.cover,
                errorBuilder: (context, error, stackTrace) =>
                    _CoverFallback(iconSize: iconSize),
              )
            : _CoverFallback(iconSize: iconSize),
      ),
    );
  }
}

class _CoverFallback extends StatelessWidget {
  final double iconSize;

  const _CoverFallback({required this.iconSize});

  @override
  Widget build(BuildContext context) {
    return Container(
      color: Colors.white24,
      child: Icon(
        Icons.music_note_rounded,
        size: iconSize,
        color: Colors.white,
      ),
    );
  }
}

class _FullContent extends StatefulWidget {
  final PlayerProvider player;

  const _FullContent({required this.player});

  @override
  State<_FullContent> createState() => _FullContentState();
}

class _FullContentState extends State<_FullContent> {
  static const Color purple = Color(0xFF9C27B0);
  static const Color purpleDark = Color(0xFF4A148C);

  bool _isDownloading = false;

  Future<void> _downloadPreview() async {
    final player = widget.player;

    if (player.isPlaying) {
      await player.pause();
    }

    final song = widget.player.currentSong;
    if (song == null) return;

    try {
      setState(() => _isDownloading = true);

      var previewUrl = song.previewUrl;

      if (previewUrl == null || previewUrl.trim().isEmpty) {
        previewUrl = await DeezerTrackProvider.getPreviewUrl(
          song.externalTrackId!,
        );
      }

      if (previewUrl == null || previewUrl.trim().isEmpty) {
        throw Exception("Song is not available for download.");
      }

      final response = await http.get(Uri.parse(previewUrl));

      if (response.statusCode < 200 || response.statusCode >= 300) {
        throw Exception("Download failed.");
      }

      final safeArtist = _safeFileName(song.artistName);
      final safeTitle = _safeFileName(song.title);
      final fileName = "$safeArtist - $safeTitle song.mp3";

      final filePath = await FilePicker.saveFile(
        dialogTitle: "Save GrooveOn song",
        fileName: fileName,
        type: FileType.custom,
        allowedExtensions: ["mp3"],
        bytes: response.bodyBytes,
      );

      if (!mounted) return;

      if (filePath == null) {
        SnackbarHelper.showInfo(context, "Download cancelled.");
        return;
      }

      SnackbarHelper.showInfo(context, "Song downloaded: $fileName");
    } catch (e) {
      if (!mounted) return;

      SnackbarHelper.showError(context, extractErrorMessage(e));
    } finally {
      if (mounted) {
        setState(() => _isDownloading = false);
      }
    }
  }

  String _safeFileName(String value) {
    return value
        .replaceAll(RegExp(r'[\\/:*?"<>|]'), "")
        .replaceAll(RegExp(r'\s+'), " ")
        .trim();
  }

  @override
  Widget build(BuildContext context) {
    final player = widget.player;
    final song = player.currentSong;

    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 24),
      child: Column(
        children: [
          _CoverArt(
            url: song?.coverUrl,
            width: double.infinity,
            height: 260,
            borderRadius: 24,
            iconSize: 80,
          ),

          const SizedBox(height: 22),

          Row(
            crossAxisAlignment: CrossAxisAlignment.center,
            children: [
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      song?.title ?? "",
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                      style: const TextStyle(
                        color: Colors.white,
                        fontSize: 21,
                        fontWeight: FontWeight.w900,
                        height: 1.1,
                      ),
                    ),
                    const SizedBox(height: 6),
                    Text(
                      song?.artistName ?? "GrooveOn",
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                      style: const TextStyle(
                        color: Colors.white70,
                        fontSize: 14,
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ],
                ),
              ),

              const SizedBox(width: 12),

              Container(
                decoration: BoxDecoration(
                  color: Colors.white.withOpacity(0.16),
                  shape: BoxShape.circle,
                  border: Border.all(color: Colors.white.withOpacity(0.24)),
                ),
                child: IconButton(
                  tooltip: "Download preview",
                  onPressed: _isDownloading ? null : _downloadPreview,
                  icon: _isDownloading
                      ? const SizedBox(
                          width: 22,
                          height: 22,
                          child: CircularProgressIndicator(
                            strokeWidth: 2,
                            color: Colors.white,
                          ),
                        )
                      : const Icon(
                          Icons.download_rounded,
                          color: Colors.white,
                          size: 27,
                        ),
                ),
              ),
            ],
          ),

          const SizedBox(height: 24),

          SliderTheme(
            data: SliderTheme.of(context).copyWith(
              trackHeight: 4,
              thumbShape: const RoundSliderThumbShape(enabledThumbRadius: 7),
              overlayShape: const RoundSliderOverlayShape(overlayRadius: 14),
              activeTrackColor: Colors.white,
              inactiveTrackColor: Colors.white24,
              thumbColor: Colors.white,
              overlayColor: Colors.white24,
            ),
            child: Slider(
              value: player.progress,
              min: 0,
              max: 1,
              onChanged: player.duration.inMilliseconds <= 0
                  ? null
                  : (value) {
                      final newPosition = Duration(
                        milliseconds: (player.duration.inMilliseconds * value)
                            .round(),
                      );

                      player.seek(newPosition);
                    },
            ),
          ),

          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                player.formatDuration(player.position),
                style: const TextStyle(
                  color: Colors.white70,
                  fontSize: 12,
                  fontWeight: FontWeight.w600,
                ),
              ),
              Text(
                player.formatDuration(player.duration),
                style: const TextStyle(
                  color: Colors.white70,
                  fontSize: 12,
                  fontWeight: FontWeight.w600,
                ),
              ),
            ],
          ),

          const SizedBox(height: 22),

          Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              IconButton(
                onPressed: player.hasPrevious ? player.playPrevious : null,
                icon: Icon(
                  Icons.skip_previous_rounded,
                  color: player.hasPrevious ? Colors.white : Colors.white30,
                  size: 42,
                ),
              ),

              const SizedBox(width: 18),

              GestureDetector(
                onTap: player.isLoading
                    ? null
                    : () {
                        if (player.isCompleted) {
                          player.repeatCurrentSong();
                        } else {
                          player.togglePlayPause();
                        }
                      },
                child: Container(
                  width: 78,
                  height: 78,
                  decoration: const BoxDecoration(
                    color: Colors.white,
                    shape: BoxShape.circle,
                  ),
                  child: player.isLoading
                      ? const Padding(
                          padding: EdgeInsets.all(24),
                          child: CircularProgressIndicator(
                            strokeWidth: 3,
                            color: purple,
                          ),
                        )
                      : Icon(
                          player.isCompleted
                              ? Icons.repeat_rounded
                              : player.isPlaying
                              ? Icons.pause_rounded
                              : Icons.play_arrow_rounded,
                          color: purpleDark,
                          size: 46,
                        ),
                ),
              ),

              const SizedBox(width: 18),

              IconButton(
                onPressed: player.hasNext ? player.playNext : null,
                icon: Icon(
                  Icons.skip_next_rounded,
                  color: player.hasNext ? Colors.white : Colors.white30,
                  size: 42,
                ),
              ),
            ],
          ),

          const SizedBox(height: 18),

          TextButton.icon(
            onPressed: player.stopPlayer,
            icon: const Icon(Icons.close_rounded, color: Colors.white70),
            label: const Text(
              "Close player",
              style: TextStyle(
                color: Colors.white70,
                fontWeight: FontWeight.w700,
              ),
            ),
          ),
        ],
      ),
    );
  }
}
