class PlayerSong {
  final int id;
  final String title;
  final String artistName;
  final int duration;
  final String? coverUrl;
  final String? externalTrackId;

  PlayerSong({
    required this.id,
    required this.title,
    required this.externalTrackId,
    required this.artistName,
    required this.duration,
    this.coverUrl,
  });
}