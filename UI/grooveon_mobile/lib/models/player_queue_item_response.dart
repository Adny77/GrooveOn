import 'package:json_annotation/json_annotation.dart';

part 'player_queue_item_response.g.dart';

@JsonSerializable()
class PlayerQueueItemResponse {
  final int id;
  final int playerId;
  final int songId;

  final int orderIndex;
  final bool isGeneratedRandomly;

  final String? songTitle;
  final String? songCoverUrl;
  final String? externalTrackId;

  PlayerQueueItemResponse({
    required this.id,
    required this.playerId,
    required this.songId,
    required this.orderIndex,
    required this.isGeneratedRandomly,
    this.songTitle,
    this.songCoverUrl,
    this.externalTrackId,
  });

  factory PlayerQueueItemResponse.fromJson(Map<String, dynamic> json) =>
      _$PlayerQueueItemResponseFromJson(json);

  Map<String, dynamic> toJson() =>
      _$PlayerQueueItemResponseToJson(this);
}