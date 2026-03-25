// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'genre_upsert_request.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

GenreUpsertRequest _$GenreUpsertRequestFromJson(Map<String, dynamic> json) =>
    GenreUpsertRequest(
      externalGenreId: json['externalGenreId'] as String,
      source: json['source'] as String? ?? "Deezer",
      name: json['name'] as String,
    );

Map<String, dynamic> _$GenreUpsertRequestToJson(GenreUpsertRequest instance) =>
    <String, dynamic>{
      'externalGenreId': instance.externalGenreId,
      'source': instance.source,
      'name': instance.name,
    };
