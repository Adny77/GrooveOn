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
        if (!player.hasSong) return const SizedBox.shrink();

        final song = player.currentSong;

        return SafeArea(
          top: false,
          child: Align(
            alignment: Alignment.bottomCenter,
            child: Padding(
              padding: const EdgeInsets.fromLTRB(14, 0, 14, 12),
              child: Material(
                elevation: 12,
                color: Colors.transparent,
                borderRadius: BorderRadius.circular(22),
                child: Container(
                  constraints: const BoxConstraints(maxWidth: 430),
                  decoration: BoxDecoration(
                    color: bg,
                    borderRadius: BorderRadius.circular(22),
                    border: Border.all(color: border),
                    boxShadow: [
                      BoxShadow(
                        color: groovePurple.withOpacity(0.12),
                        blurRadius: 20,
                        offset: const Offset(0, 8),
                      ),
                    ],
                  ),
                  child: ClipRRect(
                    borderRadius: BorderRadius.circular(22),
                    child: Column(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        /// PROGRESS BAR
                        GestureDetector(
                          behavior: HitTestBehavior.opaque,
                          onTapDown: (details) {
                            if (player.duration.inMilliseconds <= 0) return;

                            final box =
                                context.findRenderObject() as RenderBox;
                            final percent =
                                (details.localPosition.dx / box.size.width)
                                    .clamp(0.0, 1.0);

                            final newPosition = Duration(
                              milliseconds:
                                  (player.duration.inMilliseconds * percent)
                                      .round(),
                            );

                            player.seek(newPosition);
                          },
                          child: LinearProgressIndicator(
                            value: player.progress,
                            minHeight: 4,
                            backgroundColor: Colors.grey.shade300,
                            valueColor: const AlwaysStoppedAnimation<Color>(
                              groovePurple,
                            ),
                          ),
                        ),

                        /// PLAYER CONTENT
                        SizedBox(
                          height: 72,
                          child: Padding(
                            padding:
                                const EdgeInsets.symmetric(horizontal: 10),
                            child: Row(
                              children: [
                                /// COVER
                                Container(
                                  width: 46,
                                  height: 46,
                                  decoration: BoxDecoration(
                                    color: const Color(0xFFF1ECF7),
                                    borderRadius: BorderRadius.circular(14),
                                    image: song?.coverUrl != null &&
                                            song!.coverUrl!.trim().isNotEmpty
                                        ? DecorationImage(
                                            image:
                                                NetworkImage(song.coverUrl!),
                                            fit: BoxFit.cover,
                                          )
                                        : null,
                                  ),
                                  child: song?.coverUrl == null ||
                                          song!.coverUrl!.trim().isEmpty
                                      ? const Icon(
                                          Icons.music_note_rounded,
                                          color: groovePurple,
                                          size: 22,
                                        )
                                      : null,
                                ),

                                const SizedBox(width: 10),

                                /// TITLE + TIME
                                Expanded(
                                  child: Column(
                                    mainAxisAlignment:
                                        MainAxisAlignment.center,
                                    crossAxisAlignment:
                                        CrossAxisAlignment.start,
                                    children: [
                                      Text(
                                        song?.title ?? "",
                                        maxLines: 1,
                                        overflow: TextOverflow.ellipsis,
                                        style: const TextStyle(
                                          color: textPrimary,
                                          fontSize: 13.5,
                                          fontWeight: FontWeight.w800,
                                        ),
                                      ),
                                      const SizedBox(height: 3),
                                      Text(
                                        song?.artistName ?? "GrooveOn",
                                        maxLines: 1,
                                        overflow: TextOverflow.ellipsis,
                                        style: const TextStyle(
                                          color: textSecondary,
                                          fontSize: 11.5,
                                          fontWeight: FontWeight.w600,
                                        ),
                                      ),
                                      const SizedBox(height: 2),
                                      Text(
                                        "${player.formatDuration(player.position)} / ${player.formatDuration(player.duration)}",
                                        style: const TextStyle(
                                          color: textSecondary,
                                          fontSize: 10.5,
                                          fontWeight: FontWeight.w600,
                                        ),
                                      ),
                                    ],
                                  ),
                                ),

                                /// PREVIOUS
                                IconButton(
                                  visualDensity: VisualDensity.compact,
                                  padding: EdgeInsets.zero,
                                  constraints: const BoxConstraints(
                                    minWidth: 34,
                                    minHeight: 34,
                                  ),
                                  onPressed: player.hasPrevious
                                      ? () => player.playPrevious()
                                      : null,
                                  icon: Icon(
                                    Icons.skip_previous_rounded,
                                    color: player.hasPrevious
                                        ? groovePurpleDark
                                        : textSecondary.withOpacity(0.35),
                                    size: 26,
                                  ),
                                ),

                                /// PLAY / PAUSE
                                IconButton(
                                  visualDensity: VisualDensity.compact,
                                  padding: EdgeInsets.zero,
                                  constraints: const BoxConstraints(
                                    minWidth: 38,
                                    minHeight: 38,
                                  ),
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
                                          width: 18,
                                          height: 18,
                                          child: CircularProgressIndicator(
                                            strokeWidth: 2,
                                            color: groovePurple,
                                          ),
                                        )
                                      : Icon(
                                          player.isCompleted
                                              ? Icons.repeat_rounded
                                              : player.isPlaying
                                                  ? Icons
                                                      .pause_circle_filled_rounded
                                                  : Icons
                                                      .play_circle_fill_rounded,
                                          color: groovePurpleDark,
                                          size: player.isCompleted ? 28 : 31,
                                        ),
                                ),

                                /// NEXT
                                IconButton(
                                  visualDensity: VisualDensity.compact,
                                  padding: EdgeInsets.zero,
                                  constraints: const BoxConstraints(
                                    minWidth: 34,
                                    minHeight: 34,
                                  ),
                                  onPressed: player.hasNext
                                      ? () => player.playNext()
                                      : null,
                                  icon: Icon(
                                    Icons.skip_next_rounded,
                                    color: player.hasNext
                                        ? groovePurpleDark
                                        : textSecondary.withOpacity(0.35),
                                    size: 26,
                                  ),
                                ),

                                /// ❌ CLOSE PLAYER
                                IconButton(
                                  visualDensity: VisualDensity.compact,
                                  padding: EdgeInsets.zero,
                                  constraints: const BoxConstraints(
                                    minWidth: 34,
                                    minHeight: 34,
                                  ),
                                  onPressed: () {
                                    player.stopPlayer();
                                  },
                                  icon: const Icon(
                                    Icons.close_rounded,
                                    color: Colors.redAccent,
                                    size: 22,
                                  ),
                                ),
                              ],
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
              ),
            ),
          ),
        );
      },
    );
  }
}