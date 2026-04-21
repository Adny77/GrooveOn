// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'music_stat_item_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MusicStatItemResponse _$MusicStatItemResponseFromJson(
  Map<String, dynamic> json,
) => MusicStatItemResponse(
  id: (json['id'] as num).toInt(),
  title: json['title'] as String,
  imageUrl: json['imageUrl'] as String?,
  playCount: (json['playCount'] as num).toInt(),
);

Map<String, dynamic> _$MusicStatItemResponseToJson(
  MusicStatItemResponse instance,
) => <String, dynamic>{
  'id': instance.id,
  'title': instance.title,
  'imageUrl': instance.imageUrl,
  'playCount': instance.playCount,
};
