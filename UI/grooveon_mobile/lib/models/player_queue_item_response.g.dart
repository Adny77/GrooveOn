// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'player_queue_item_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PlayerQueueItemResponse _$PlayerQueueItemResponseFromJson(
  Map<String, dynamic> json,
) => PlayerQueueItemResponse(
  id: (json['id'] as num).toInt(),
  playerId: (json['playerId'] as num).toInt(),
  songId: (json['songId'] as num).toInt(),
  orderIndex: (json['orderIndex'] as num).toInt(),
  isGeneratedRandomly: json['isGeneratedRandomly'] as bool,
  songTitle: json['songTitle'] as String?,
  songCoverUrl: json['songCoverUrl'] as String?,
  externalTrackId: json['externalTrackId'] as String?,
);

Map<String, dynamic> _$PlayerQueueItemResponseToJson(
  PlayerQueueItemResponse instance,
) => <String, dynamic>{
  'id': instance.id,
  'playerId': instance.playerId,
  'songId': instance.songId,
  'orderIndex': instance.orderIndex,
  'isGeneratedRandomly': instance.isGeneratedRandomly,
  'songTitle': instance.songTitle,
  'songCoverUrl': instance.songCoverUrl,
  'externalTrackId': instance.externalTrackId,
};
