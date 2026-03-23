import 'package:flutter/material.dart';
import 'package:grooveon_desktop/dialogs/base_dialogs_frame.dart';
import 'package:grooveon_desktop/screens/music_screen.dart';

class AlbumTrackPreview {
  final String title;
  final String duration;
  final bool alreadyExists;

  AlbumTrackPreview({
    required this.title,
    required this.duration,
    required this.alreadyExists,
  });
}

class AlbumAddPreviewDialog extends StatelessWidget {
  final String albumTitle;
  final String artistName;
  final String? coverUrl;
  final List<AlbumTrackPreview> tracks;
  final VoidCallback onContinue;

  const AlbumAddPreviewDialog({
    super.key,
    required this.albumTitle,
    required this.artistName,
    this.coverUrl,
    required this.tracks,
    required this.onContinue,
  });

  @override
  Widget build(BuildContext context) {
    final toAddCount = tracks.where((x) => !x.alreadyExists).length;
    final existingCount = tracks.where((x) => x.alreadyExists).length;

    return BaseDialog(
      title: "Album Preview",
      width: 760,
      height: 620,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _AlbumHeader(
            albumTitle: albumTitle,
            artistName: artistName,
            coverUrl: coverUrl,
            trackCount: tracks.length,
            toAddCount: toAddCount,
            existingCount: existingCount,
          ),
          const SizedBox(height: 20),
          const Text(
            "Tracks that will be processed",
            style: TextStyle(
              fontSize: 16,
              fontWeight: FontWeight.w800,
              color: MusicScreen.textColor,
            ),
          ),
          const SizedBox(height: 12),
          Expanded(
            child: Container(
              decoration: BoxDecoration(
                border: Border.all(color: MusicScreen.borderColor),
                borderRadius: BorderRadius.circular(14),
              ),
              child: ListView.separated(
                padding: const EdgeInsets.all(14),
                itemCount: tracks.length,
                separatorBuilder: (_, __) => const SizedBox(height: 10),
                itemBuilder: (context, index) {
                  final track = tracks[index];
                  return _TrackPreviewTile(
                    index: index + 1,
                    title: track.title,
                    duration: track.duration,
                    alreadyExists: track.alreadyExists,
                  );
                },
              ),
            ),
          ),
          const SizedBox(height: 18),
          Row(
            children: [
              Expanded(
                child: OutlinedButton(
                  onPressed: () => Navigator.of(context).pop(),
                  style: OutlinedButton.styleFrom(
                    minimumSize: const Size.fromHeight(46),
                    side: const BorderSide(color: MusicScreen.borderColor),
                  ),
                  child: const Text("Cancel"),
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: ElevatedButton(
                  onPressed: onContinue,
                  style: ElevatedButton.styleFrom(
                    minimumSize: const Size.fromHeight(46),
                    backgroundColor: MusicScreen.primaryColor,
                    foregroundColor: Colors.white,
                    elevation: 0,
                  ),
                  child: const Text("Continue"),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}

class _AlbumHeader extends StatelessWidget {
  final String albumTitle;
  final String artistName;
  final String? coverUrl;
  final int trackCount;
  final int toAddCount;
  final int existingCount;

  const _AlbumHeader({
    required this.albumTitle,
    required this.artistName,
    required this.coverUrl,
    required this.trackCount,
    required this.toAddCount,
    required this.existingCount,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: MusicScreen.primaryLight,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: MusicScreen.borderColor),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            width: 92,
            height: 92,
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(12),
              border: Border.all(color: MusicScreen.borderColor),
              image: coverUrl != null && coverUrl!.isNotEmpty
                  ? DecorationImage(
                      image: NetworkImage(coverUrl!),
                      fit: BoxFit.cover,
                    )
                  : null,
            ),
            child: (coverUrl == null || coverUrl!.isEmpty)
                ? const Icon(
                    Icons.album_rounded,
                    size: 40,
                    color: MusicScreen.primaryColor,
                  )
                : null,
          ),
          const SizedBox(width: 16),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  albumTitle,
                  style: const TextStyle(
                    fontSize: 20,
                    fontWeight: FontWeight.w800,
                    color: MusicScreen.textColor,
                  ),
                ),
                const SizedBox(height: 6),
                Text(
                  artistName,
                  style: const TextStyle(
                    fontSize: 14,
                    fontWeight: FontWeight.w600,
                    color: MusicScreen.subTextColor,
                  ),
                ),
                const SizedBox(height: 12),
                Wrap(
                  spacing: 8,
                  runSpacing: 8,
                  children: [
                    _HeaderChip(label: "$trackCount tracks"),
                    _HeaderChip(label: "$toAddCount new"),
                    _HeaderChip(label: "$existingCount existing"),
                  ],
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _HeaderChip extends StatelessWidget {
  final String label;

  const _HeaderChip({required this.label});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(999),
        border: Border.all(color: MusicScreen.borderColor),
      ),
      child: Text(
        label,
        style: const TextStyle(
          fontSize: 12,
          fontWeight: FontWeight.w700,
          color: MusicScreen.textColor,
        ),
      ),
    );
  }
}

class _TrackPreviewTile extends StatelessWidget {
  final int index;
  final String title;
  final String duration;
  final bool alreadyExists;

  const _TrackPreviewTile({
    required this.index,
    required this.title,
    required this.duration,
    required this.alreadyExists,
  });

  @override
  Widget build(BuildContext context) {
    final badgeColor = alreadyExists
        ? const Color(0xFFE8F5E9)
        : const Color(0xFFEDE7F6);

    final badgeTextColor = alreadyExists
        ? const Color(0xFF2E7D32)
        : MusicScreen.primaryColor;

    final badgeText = alreadyExists ? "Already exists" : "Will be added";

    return Container(
      padding: const EdgeInsets.all(13),
      decoration: BoxDecoration(
        color: const Color(0xFFFCFCFD),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: MusicScreen.borderColor),
      ),
      child: Row(
        children: [
          Container(
            width: 28,
            height: 28,
            alignment: Alignment.center,
            decoration: BoxDecoration(
              color: MusicScreen.primaryLight,
              borderRadius: BorderRadius.circular(8),
            ),
            child: Text(
              "$index",
              style: const TextStyle(
                fontSize: 12,
                fontWeight: FontWeight.w700,
                color: MusicScreen.primaryColor,
              ),
            ),
          ),
          const SizedBox(width: 12),
          const Icon(
            Icons.music_note_rounded,
            color: MusicScreen.primaryColor,
            size: 18,
          ),
          const SizedBox(width: 10),
          Expanded(
            child: Text(
              title,
              style: const TextStyle(
                fontSize: 13,
                fontWeight: FontWeight.w700,
                color: MusicScreen.textColor,
              ),
            ),
          ),
          Text(
            duration,
            style: const TextStyle(
              fontSize: 12,
              color: MusicScreen.subTextColor,
            ),
          ),
          const SizedBox(width: 12),
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
            decoration: BoxDecoration(
              color: badgeColor,
              borderRadius: BorderRadius.circular(999),
            ),
            child: Text(
              badgeText,
              style: TextStyle(
                fontSize: 11,
                fontWeight: FontWeight.w700,
                color: badgeTextColor,
              ),
            ),
          ),
        ],
      ),
    );
  }
}