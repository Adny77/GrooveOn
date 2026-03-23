// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'song_bulk_insert_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SongBulkInsertResponse _$SongBulkInsertResponseFromJson(
  Map<String, dynamic> json,
) => SongBulkInsertResponse(
  savedCount: (json['savedCount'] as num).toInt(),
  savedSongIds: (json['savedSongIds'] as List<dynamic>)
      .map((e) => (e as num).toInt())
      .toList(),
);

Map<String, dynamic> _$SongBulkInsertResponseToJson(
  SongBulkInsertResponse instance,
) => <String, dynamic>{
  'savedCount': instance.savedCount,
  'savedSongIds': instance.savedSongIds,
};
