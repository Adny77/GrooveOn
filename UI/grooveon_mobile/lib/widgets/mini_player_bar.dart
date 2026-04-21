import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../providers/player_provider.dart';

class MiniPlayerBar extends StatelessWidget {
  const MiniPlayerBar({super.key});

  static const Color groovePurple = Color(0xFF9C27B0);
  static const Color groovePurpleDark = Color(0xFF4A148C);
  static const Color bg = Color(0xFFFFFFFF);
  static const Color border = Color(0xFFE6E6EF);
  static const Color textPrimary = Color(0xFF1C1C1C);
  static const Color textSecondary = Color(0xFF7A7A85);

  @override
  Widget build(BuildContext context) {
    return Consumer<PlayerProvider>(
      builder: (context, player, child) {
        if (!player.isVisible || !player.hasSong) {
          return const SizedBox.shrink();
        }

        return SafeArea(
          top: false,
          child: Padding(
            padding: const EdgeInsets.fromLTRB(12, 0, 12, 10),
            child: Material(
              elevation: 10,
              color: Colors.transparent,
              borderRadius: BorderRadius.circular(18),
              child: Container(
                decoration: BoxDecoration(
                  color: bg,
                  borderRadius: BorderRadius.circular(18),
                  border: Border.all(color: border),
                  boxShadow: [
                    BoxShadow(
                      color: groovePurple.withOpacity(0.08),
                      blurRadius: 18,
                      offset: const Offset(0, 8),
                    ),
                  ],
                ),
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    LinearProgressIndicator(
                      value: player.progress,
                      minHeight: 3,
                      backgroundColor: Colors.transparent,
                      valueColor:
                          const AlwaysStoppedAnimation<Color>(groovePurple),
                      borderRadius: const BorderRadius.vertical(
                        top: Radius.circular(18),
                      ),
                    ),
                    Padding(
                      padding: const EdgeInsets.fromLTRB(10, 10, 10, 10),
                      child: Row(
                        children: [
                          // COVER
                          Container(
                            width: 52,
                            height: 52,
                            decoration: BoxDecoration(
                              color: const Color(0xFFF1ECF7),
                              borderRadius: BorderRadius.circular(12),
                              image: player.currentCover != null &&
                                      player.currentCover!.trim().isNotEmpty
                                  ? DecorationImage(
                                      image: NetworkImage(player.currentCover!),
                                      fit: BoxFit.cover,
                                    )
                                  : null,
                            ),
                            child: player.currentCover == null ||
                                    player.currentCover!.trim().isEmpty
                                ? const Icon(
                                    Icons.music_note_rounded,
                                    color: groovePurple,
                                  )
                                : null,
                          ),

                          const SizedBox(width: 12),

                          // TITLE + ARTIST
                          Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text(
                                  player.currentTitle,
                                  maxLines: 1,
                                  overflow: TextOverflow.ellipsis,
                                  style: const TextStyle(
                                    color: textPrimary,
                                    fontSize: 14,
                                    fontWeight: FontWeight.w800,
                                  ),
                                ),
                                const SizedBox(height: 3),
                                Text(
                                  player.currentArtist,
                                  maxLines: 1,
                                  overflow: TextOverflow.ellipsis,
                                  style: const TextStyle(
                                    color: textSecondary,
                                    fontSize: 12,
                                    fontWeight: FontWeight.w600,
                                  ),
                                ),
                                const SizedBox(height: 5),
                                Text(
                                  "${player.formatDuration(player.position)} / ${player.formatDuration(player.duration)}",
                                  style: const TextStyle(
                                    color: textSecondary,
                                    fontSize: 11,
                                    fontWeight: FontWeight.w600,
                                  ),
                                ),
                              ],
                            ),
                          ),

                          const SizedBox(width: 6),

                          // // ⏮️ PREVIOUS
                          // IconButton(
                          //   onPressed: player.isLoading
                          //       ? null
                          //       : () => player.playPrevious(),
                          //   icon: const Icon(
                          //     Icons.skip_previous_rounded,
                          //     color: groovePurpleDark,
                          //     size: 30,
                          //   ),
                          // ),

                          // ▶️ / ⏸️ PLAY PAUSE
                          IconButton(
                            onPressed: player.isLoading
                                ? null
                                : () => player.togglePlayPause(),
                            icon: player.isLoading
                                ? const SizedBox(
                                    width: 22,
                                    height: 22,
                                    child: CircularProgressIndicator(
                                      strokeWidth: 2,
                                      color: groovePurple,
                                    ),
                                  )
                                : Icon(
                                    player.isPlaying
                                        ? Icons.pause_circle_filled_rounded
                                        : Icons.play_circle_fill_rounded,
                                    color: groovePurpleDark,
                                    size: 34,
                                  ),
                          ),

                          // ⏭️ NEXT
                          // IconButton(
                          //   onPressed: player.isLoading
                          //       ? null
                          //       : () => player.playNext(),
                          //   icon: const Icon(
                          //     Icons.skip_next_rounded,
                          //     color: groovePurpleDark,
                          //     size: 30,
                          //   ),
                          // ),

                          // ❌ CLOSE
                          IconButton(
                            onPressed: () => player.closePlayer(),
                            icon: const Icon(
                              Icons.close_rounded,
                              color: textSecondary,
                            ),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ),
        );
      },
    );
  }
}