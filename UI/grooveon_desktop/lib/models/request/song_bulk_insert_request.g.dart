// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'song_bulk_insert_request.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SongBulkInsertRequest _$SongBulkInsertRequestFromJson(
  Map<String, dynamic> json,
) => SongBulkInsertRequest(
  songs: (json['songs'] as List<dynamic>)
      .map((e) => SongUpsertRequest.fromJson(e as Map<String, dynamic>))
      .toList(),
);

Map<String, dynamic> _$SongBulkInsertRequestToJson(
  SongBulkInsertRequest instance,
) => <String, dynamic>{'songs': instance.songs.map((e) => e.toJson()).toList()};
