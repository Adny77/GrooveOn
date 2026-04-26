import 'package:json_annotation/json_annotation.dart';

part 'player_response.g.dart';

@JsonSerializable()
class PlayerResponse {
  final int id;
  final int songId;
  final String title;
  final String artistName;
  final int duration;
  final String coverUrl;
  final String externalTrackId;
  final bool hasPrevious;
  final bool hasNext;

  PlayerResponse({
    required this.id,
    required this.songId,
    required this.title,
    required this.artistName,
    required this.duration,
    required this.coverUrl,
    required this.externalTrackId,
    required this.hasPrevious,
    required this.hasNext,
  });

  factory PlayerResponse.fromJson(Map<String, dynamic> json) =>
      _$PlayerResponseFromJson(json);

  Map<String, dynamic> toJson() => _$PlayerResponseToJson(this);
}
